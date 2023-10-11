namespace YDotNet.Infrastructure;

/// <summary>
/// Marker interface for resource owners.
/// </summary>
public interface IResourceOwner
{
    /// <summary>
    /// Gets a value indicating whether this instance is disposed.
    /// </summary>
    bool IsDisposed { get; }
}
