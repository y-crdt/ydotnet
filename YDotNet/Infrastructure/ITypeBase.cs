namespace YDotNet.Infrastructure;

/// <summary>
///     Base class for all managed types.
/// </summary>
public interface ITypeBase
{
    /// <summary>
    ///     Gets a value indicating whether the instance is disposed.
    /// </summary>
    bool IsDisposed { get; }

    /// <summary>
    ///     Marks the object as disposed to stop all further calls.
    /// </summary>
    void MarkDisposed();
}
