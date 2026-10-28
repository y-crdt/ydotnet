using YDotNet.Benchmarks.Memory;

// Usage: dotnet run -c Release --project Benchmarks/YDotNet.Benchmarks.Memory -- [filter] [--iterations N] [--batches N]
var filter = args.FirstOrDefault(x => !x.StartsWith("--", StringComparison.Ordinal) && !int.TryParse(x, out _));
var options = new LeakDetectorOptions(
    Iterations: ReadOption("--iterations", LeakDetectorOptions.Default.Iterations),
    Batches: ReadOption("--batches", LeakDetectorOptions.Default.Batches));

if (!NativeHeap.IsSupported)
{
    Console.Error.WriteLine("The native heap cannot be measured on this platform (supported: macOS, Linux with glibc).");
    return 2;
}

var scenarios = Scenarios.All
    .Where(x => filter == null || x.Id.Contains(filter, StringComparison.OrdinalIgnoreCase))
    .ToList();

Console.WriteLine(
    $"Running {scenarios.Count} scenarios, {options.Iterations} iterations x {options.Batches} batches, " +
    $"threshold {options.NativeThresholdBytesPerIteration} B/iteration native, {options.ManagedThresholdBytesPerIteration} managed.");
Console.WriteLine();
Console.WriteLine($"{"Scenario",-52} {"Native B/it",12} {"Managed B/it",13}  Result");

var failures = 0;

foreach (var scenario in scenarios)
{
    // Written before running so that a native crash still shows which scenario caused it.
    Console.Write($"{scenario.Id,-52} ");

    LeakReport report;
    try
    {
        report = LeakDetector.Run(scenario, options);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"{"-",12} {"-",13}  ERROR {ex.GetType().Name}: {ex.Message}");
        failures++;
        continue;
    }

    var result = (report.Leaks, scenario.KnownLeak) switch
    {
        (true, not null) => "known leak",
        (true, null) => "LEAK",
        (false, not null) => "FIXED (known leak no longer reproduces, remove KnownLeak)",
        _ => "ok",
    };

    // A known leak that no longer reproduces fails too, so that the scenario goes back to being checked.
    if (report.Leaks == (scenario.KnownLeak == null))
    {
        failures++;
    }

    Console.WriteLine(
        $"{report.NativeBytesPerIteration,12:0.0} {report.ManagedBytesPerIteration,13:0.0}  {result}");
}

Console.WriteLine();
Console.WriteLine(failures == 0 ? "No leaks found." : $"{failures} scenario(s) leaking, failing or no longer leaking as known.");

return failures == 0 ? 0 : 1;

int ReadOption(string name, int defaultValue)
{
    var index = Array.IndexOf(args, name);

    return index >= 0 && index + 1 < args.Length ? int.Parse(args[index + 1]) : defaultValue;
}
