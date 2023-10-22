namespace YDotNet.Infrastructure;

internal class TypeCache
{
    private readonly Dictionary<nint, WeakReference<ITypeBase>> cache = new();

    public T GetOrAdd<T>(nint handle, Func<nint, T> factory)
        where T : ITypeBase
    {
        if (handle == nint.Zero)
        {
            throw new ArgumentException("Cannot create object for null handle.", nameof(handle));
        }

        Cleanup();

        if (cache.TryGetValue(handle, out var weakRef) && weakRef.TryGetTarget(out var item))
        {
            if (item is not T typed)
            {
                throw new YDotNetException($"Expected {typeof(T)}, got {item.GetType()}");
            }

            return typed;
        }

        var typedItem = factory(handle);

        cache[handle] = new WeakReference<ITypeBase>(typedItem);

        return typedItem;
    }

    // TODO [LSViana] Check with Sebastian why this method isn't used anywhere.
    public void Delete(nint handle)
    {
        if (cache.TryGetValue(handle, out var weakRef))
        {
            if (weakRef.TryGetTarget(out var item))
            {
                item.MarkDisposed();
            }

            cache.Remove(handle);
        }
    }

    private void Cleanup()
    {
        List<nint>? keysToDelete = null;

        foreach (var (key, weakRef) in cache)
        {
            if (!weakRef.TryGetTarget(out _))
            {
                keysToDelete ??= new List<nint>();
                keysToDelete.Add(key);
            }
        }

        if (keysToDelete != null)
        {
            foreach (var key in keysToDelete)
            {
                cache.Remove(key);
            }
        }
    }
}
