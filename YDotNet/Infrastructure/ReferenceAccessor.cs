using YDotNet.Document.Transactions;

namespace YDotNet.Infrastructure;

/// <summary>
///     Used to translate native pointers to references.
/// </summary>
public class ReferenceAccessor
{
    /// <summary>
    ///     Accesses a <see cref="Transaction" /> instance based on <see cref="Transaction.Handle" />.
    /// </summary>
    /// <param name="instance">The <see cref="Transaction" /> instance to be returned.</param>
    /// <returns>
    ///     The same instance provided or <c>null</c> if the internal pointer is <see cref="nint.Zero" />.
    /// </returns>
    public Transaction? Access(Transaction instance)
    {
        return Access(instance, instance.Handle);
    }

    private T? Access<T>(T instance, IntPtr pointer)
        where T : class
    {
        return pointer == nint.Zero ? null : instance;
    }
}
