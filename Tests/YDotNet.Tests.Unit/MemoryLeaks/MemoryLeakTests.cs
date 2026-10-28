using NUnit.Framework;
using YDotNet.Benchmarks.Memory;

namespace YDotNet.Tests.Unit.MemoryLeaks;

/// <summary>
///     Repeats each operation from <see cref="Scenarios" /> and fails if native or managed memory keeps growing.
/// </summary>
/// <remarks>
///     Run them with <c>dotnet test --filter Category=Memory</c>. They measure the whole process, so they must not run
///     in parallel with other tests.
/// </remarks>
[Category("Memory")]
[NonParallelizable]
public class MemoryLeakTests
{
    private static readonly LeakDetectorOptions Options = new(Iterations: 2_000, Batches: 5, WarmupBatches: 3);

    [SetUp]
    public void SetUp()
    {
        if (!NativeHeap.IsSupported)
        {
            Assert.Ignore("The native heap cannot be measured on this platform.");
        }
    }

    [Test]
    public void DetectsKnownNativeLeak()
    {
        var report = LeakDetector.Run(Scenarios.KnownNativeLeak, Options);

        Assert.That(report.LeaksNative, Is.True, report.ToString());
        Assert.That(report.NativeBytesPerIteration, Is.GreaterThan(48), report.ToString());
    }

    [Test]
    public void DetectsKnownManagedLeak()
    {
        var report = LeakDetector.Run(Scenarios.KnownManagedLeak, Options);

        Assert.That(report.LeaksManaged, Is.True, report.ToString());
        Assert.That(report.LeaksNative, Is.False, report.ToString());
    }

    [TestCaseSource(nameof(AllScenarios))]
    public void DoesNotLeak(MemoryScenario scenario)
    {
        var report = LeakDetector.Run(scenario, Options);

        if (scenario.KnownLeak != null)
        {
            // Fails once the leak is fixed upstream, so that the scenario goes back to being checked.
            Assert.That(
                report.Leaks,
                Is.True,
                $"The known leak no longer reproduces, remove KnownLeak from {scenario.Id}: {scenario.KnownLeak}\n{report}");

            Assert.Ignore(scenario.KnownLeak);
        }

        Assert.That(report.Leaks, Is.False, report.ToString());
    }

    private static IEnumerable<TestCaseData> AllScenarios()
    {
        return Scenarios.All.Select(x => new TestCaseData(x).SetName($"DoesNotLeak({x.Id})"));
    }
}
