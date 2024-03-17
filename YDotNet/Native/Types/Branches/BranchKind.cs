using YDotNet.Document.Cells;

namespace YDotNet.Native.Types.Branches;

internal enum BranchKind
{
    Null = OutputTag.Null,
    Array = OutputTag.Array,
    Map = OutputTag.Map,
    Text = OutputTag.Text,
    XmlElement = OutputTag.XmlElement,
    XmlText = OutputTag.XmlText,
    XmlFragment = OutputTag.XmlFragment
}
