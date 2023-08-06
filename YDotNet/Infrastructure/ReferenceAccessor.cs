using YDotNet.Document;
using YDotNet.Document.Cells;
using YDotNet.Document.Transactions;
using YDotNet.Document.Types;
using YDotNet.Document.Types.Arrays;
using YDotNet.Document.Types.Maps;
using YDotNet.Document.Types.Texts;
using Array = YDotNet.Document.Types.Arrays.Array;

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

    public static MapIterator? Access(MapIterator instance)
    {
        return Access(instance, instance.Handle);
    }

    public static Array? Access(Array instance)
    {
        return Access(instance, instance.Handle);
    }

    public static ArrayIterator? Access(ArrayIterator instance)
    {
        return Access(instance, instance.Handle);
    }

    public static Text? Access(Text instance)
    {
        return Access(instance, instance.Handle);
    }

    public static XmlElement? Access(XmlElement instance)
    {
        return Access(instance, instance.Handle);
    }

    public static XmlText? Access(XmlText instance)
    {
        return Access(instance, instance.Handle);
    }

    public static Doc? Access(Doc instance)
    {
        return Access(instance, instance.Handle);
    }

    public static Output? Access(Output instance)
    {
        return Access(instance, instance.Handle);
    }

    private static T? Access<T>(T instance, nint pointer)
        where T : class
    {
        return pointer == nint.Zero ? null : instance;
    }
}
