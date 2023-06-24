using YDotNet.Document;
using YDotNet.Document.Transactions;
using YDotNet.Document.Types;

namespace YDotNet.Infrastructure;

internal static class ReferenceAccessor
{
    public static Transaction? Access(Transaction instance)
    {
        return Access(instance, instance.Handle);
    }

    public static Map? Access(Map instance)
    {
        return Access(instance, instance.Handle);
    }

    public static Text? Access(Text instance)
    {
        return Access(instance, instance.Handle);
    }

    public static Doc? Access(Doc instance)
    {
        return Access(instance, instance.Handle);
    }

    private static T? Access<T>(T instance, IntPtr pointer)
        where T : class
    {
        return pointer == nint.Zero ? null : instance;
    }
}
