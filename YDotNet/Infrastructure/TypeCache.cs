namespace YDotNet.Infrastructure;

internal class TypeCache
{
    private readonly Dictionary<nint, ITypeBase> cache = new Dictionary<nint, ITypeBase>();

    public T GetOrAdd<T>(nint handle, Func<nint, T> factory)
        where T : ITypeBase
    {
        if (handle == nint.Zero)
        {
            ThrowHelper.ArgumentException("Cannot create object for null handle.", nameof(handle));
        }

        if (cache.TryGetValue(handle, out var item))
        {
            if (item is not T typed)
            {
                ThrowHelper.YDotnet($"Expected {typeof(T)}, got {item.GetType()}");
                return default!;
            }

            return typed;
        }

        var typedItem = factory(handle);
        cache[handle] = typedItem;

        return typedItem;
    }

    public void Delete(nint handle)
    {
        if (cache.TryGetValue(handle, out var item))
        {
            item.MarkDeleted();
            cache.Remove(handle);
        }
    }
}
