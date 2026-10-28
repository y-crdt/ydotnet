namespace YDotNet.Benchmarks.Memory;

/// <summary>
///     Settings for <see cref="LeakDetector" />.
/// </summary>
/// <param name="Iterations">How many times the operation runs in each batch.</param>
/// <param name="Batches">How many batches are measured after the warm-up batches.</param>
/// <param name="NativeThresholdBytesPerIteration">
///     The native growth per iteration above which the scenario is reported as leaking. The smallest native
///     allocation is 16 bytes, so leaking a single allocation per iteration is above the default.
/// </param>
/// <param name="WarmupBatches">
///     How many batches run before measuring, to fill caches and finish lazy initialization and JIT compilation.
///     Must be at least 1.
/// </param>
/// <param name="ManagedThresholdBytesPerIteration">
///     The managed growth per iteration above which the scenario is reported as leaking. It is higher than the native
///     one because a test runner keeps objects alive while it reports results, and leaked wrappers, delegates and
///     closures take more than that anyway.
/// </param>
public sealed record LeakDetectorOptions(
    int Iterations = 2_000,
    int Batches = 5,
    double NativeThresholdBytesPerIteration = 12,
    int WarmupBatches = 2,
    double ManagedThresholdBytesPerIteration = 32)
{
    /// <summary>
    ///     Gets the default settings.
    /// </summary>
    public static LeakDetectorOptions Default { get; } = new();
}

/// <summary>
///     The memory measured while running a <see cref="MemoryScenario" />.
/// </summary>
/// <param name="Scenario">The scenario.</param>
/// <param name="Options">The settings used to run it.</param>
/// <param name="NativeSamples">Bytes in the native heap after the warm-up and after each measured batch.</param>
/// <param name="ManagedSamples">Bytes in the managed heap after the warm-up and after each measured batch.</param>
public sealed record LeakReport(
    MemoryScenario Scenario,
    LeakDetectorOptions Options,
    IReadOnlyList<long> NativeSamples,
    IReadOnlyList<long> ManagedSamples)
{
    /// <summary>
    ///     Gets the growth of the native heap per iteration, as the median of the growth of each batch.
    /// </summary>
    public double NativeBytesPerIteration => MedianGrowth(NativeSamples);

    /// <summary>
    ///     Gets the growth of the managed heap per iteration, as the median of the growth of each batch.
    /// </summary>
    public double ManagedBytesPerIteration => MedianGrowth(ManagedSamples);

    /// <summary>
    ///     Gets a value indicating whether the native heap grew more than the threshold.
    /// </summary>
    public bool LeaksNative => NativeBytesPerIteration > Options.NativeThresholdBytesPerIteration;

    /// <summary>
    ///     Gets a value indicating whether the managed heap grew more than the threshold.
    /// </summary>
    public bool LeaksManaged => ManagedBytesPerIteration > Options.ManagedThresholdBytesPerIteration;

    /// <summary>
    ///     Gets a value indicating whether the scenario leaks memory.
    /// </summary>
    public bool Leaks => LeaksNative || LeaksManaged;

    // A leak grows the heap in every batch, while the runtime allocates once in a while (JIT, thread pool, caches).
    // The median ignores those one-off allocations.
    private double MedianGrowth(IReadOnlyList<long> samples)
    {
        var growths = samples.Zip(samples.Skip(1), (before, after) => after - before).Order().ToList();

        var median = growths.Count % 2 == 1
            ? growths[growths.Count / 2]
            : (growths[(growths.Count / 2) - 1] + growths[growths.Count / 2]) / 2.0;

        return median / Options.Iterations;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{Scenario.Id}: native {NativeBytesPerIteration:0.0} B/iteration, managed {ManagedBytesPerIteration:0.0} B/iteration " +
               $"(native samples: {string.Join(", ", NativeSamples)}; managed samples: {string.Join(", ", ManagedSamples)})";
    }
}

/// <summary>
///     Repeats an operation in batches and records how the native and managed heaps change between batches.
/// </summary>
/// <remarks>
///     The warm-up batches fill caches and finish lazy initialization and JIT compilation and are not counted. Memory that is still
///     growing after it, with all collectable objects collected and finalized, is memory that is never released.
/// </remarks>
public static class LeakDetector
{
    /// <summary>
    ///     Runs a scenario and measures its memory usage.
    /// </summary>
    /// <param name="scenario">The scenario to run.</param>
    /// <param name="options">The settings, or <c>null</c> for the default ones.</param>
    /// <returns>The measurements.</returns>
    public static LeakReport Run(MemoryScenario scenario, LeakDetectorOptions? options = null)
    {
        options ??= LeakDetectorOptions.Default;

        var nativeSamples = new List<long>();
        var managedSamples = new List<long>();

        using (var context = new ScenarioContext())
        {
            var action = scenario.Prepare(context);

            for (var batch = 0; batch < options.WarmupBatches + options.Batches; batch++)
            {
                for (var i = 0; i < options.Iterations; i++)
                {
                    action();
                }

                Settle();

                if (batch >= options.WarmupBatches - 1)
                {
                    nativeSamples.Add(NativeHeap.BytesInUse());
                    managedSamples.Add(LiveManagedBytes());
                }
            }
        }

        Settle();

        return new LeakReport(scenario, options, nativeSamples, managedSamples);
    }

    // The bytes that survived the last full collection, which Settle() just ran. Unlike GC.GetTotalMemory(), this does
    // not count what other threads allocated since then, such as the test runner reporting results.
    private static long LiveManagedBytes()
    {
        return GC.GetGCMemoryInfo(GCKind.FullBlocking).PromotedBytes;
    }

    private static void Settle()
    {
        // Objects released by finalizers become collectable only on the next collection.
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }
}
