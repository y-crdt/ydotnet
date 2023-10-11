using System.Collections;

namespace YDotNet.Infrastructure;

public abstract class UnmanagedCollectionResource<T> : UnmanagedResource, IReadOnlyList<T>
{
    private readonly List<T> items = new List<T>();

    public UnmanagedCollectionResource(nint handle, IResourceOwner? owner)
        : base(handle, owner)
    {
    }

    /// <inheritdoc />
    public T this[int index] => items[index];

    /// <inheritdoc />
    public int Count => items.Count;

    protected void AddItem(T item)
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
