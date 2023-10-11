using System.Collections;

namespace YDotNet.Infrastructure;

/// <summary>
/// Base class for resources that represent a collection.
/// </summary>
/// <typeparam name="T">The type of item.</typeparam>
public abstract class UnmanagedCollectionResource<T> : UnmanagedResource, IReadOnlyList<T>
{
    private readonly List<T> items = new();

    internal UnmanagedCollectionResource(nint handle, IResourceOwner? owner)
        : base(handle, owner)
    {
    }

    /// <inheritdoc />
    public T this[int index] => items[index];

    /// <inheritdoc />
    public int Count => items.Count;

    /// <summary>
    /// Adds a new item to the collection.
    /// </summary>
    /// <param name="item">The item to add.</param>
    protected internal void AddItem(T item)
    {
        items.Add(item);
    }

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator()
    {
        return items.GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return items.GetEnumerator();
    }
}
