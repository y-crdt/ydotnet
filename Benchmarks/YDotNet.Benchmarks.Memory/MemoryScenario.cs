namespace YDotNet.Benchmarks.Memory;

/// <summary>
///     An operation that is repeated many times to check whether it leaks memory.
/// </summary>
/// <param name="Group">The type the operation belongs to, such as <c>Map</c>.</param>
/// <param name="Name">What the operation does.</param>
/// <param name="Prepare">
///     Creates the state shared by all iterations, registering what must be disposed at the end in the context, and
///     returns the operation to repeat.
/// </param>
public sealed record MemoryScenario(string Group, string Name, Func<ScenarioContext, Action> Prepare)
{
    /// <summary>
    ///     Gets the identifier of the scenario, such as <c>Map/Insert</c>.
    /// </summary>
    public string Id => $"{Group}/{Name}";

    /// <summary>
    ///     Gets the reason why the scenario leaks and cannot be fixed in YDotNet, or <c>null</c> if it must not leak.
    /// </summary>
    public string? KnownLeak { get; init; }

    /// <inheritdoc />
    public override string ToString()
    {
        return Id;
    }
}

/// <summary>
///     Holds the state shared by all iterations of a <see cref="MemoryScenario" />.
/// </summary>
public sealed class ScenarioContext : IDisposable
{
    private readonly Stack<IDisposable> resources = new();

    /// <summary>
    ///     Registers a resource to be disposed when the scenario ends.
    /// </summary>
    /// <typeparam name="T">The type of the resource.</typeparam>
    /// <param name="resource">The resource.</param>
    /// <returns>The same resource.</returns>
    public T Track<T>(T resource)
        where T : IDisposable
    {
        resources.Push(resource);

        return resource;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        while (resources.TryPop(out var resource))
        {
            resource.Dispose();
        }
    }
}
