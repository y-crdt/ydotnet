using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;

namespace YDotNet.Tests.Unit.Document;

/// <summary>
///     Out-of-range indexes must surface as exceptions. The native library panics instead, which aborts the process.
/// </summary>
public class IndexBoundsTests
{
    private static Input Attributes()
    {
        return Input.Object(new Dictionary<string, Input> { ["bold"] = Input.Boolean(value: true) });
    }

    [Test]
    public void ArrayInsertRangePastEndThrows()
    {
        var doc = new Doc();
        var array = doc.Array("array");

        using var transaction = doc.WriteTransaction();

        Assert.Throws<ArgumentOutOfRangeException>(() => array.InsertRange(transaction, index: 1, Input.Long(value: 1)));
    }

    [Test]
    public void ArrayInsertRangeAtEndAppends()
    {
        var doc = new Doc();
        var array = doc.Array("array");

        using var transaction = doc.WriteTransaction();

        array.InsertRange(transaction, index: 0, Input.Long(value: 1));
        array.InsertRange(transaction, index: 1, Input.Long(value: 2));

        Assert.That(array.Length(transaction), Is.EqualTo(expected: 2));
    }

    [Test]
    public void ArrayRemoveRangePastEndThrows()
    {
        var doc = new Doc();
        var array = doc.Array("array");

        using var transaction = doc.WriteTransaction();

        array.InsertRange(transaction, index: 0, Input.Long(value: 1), Input.Long(value: 2));

        Assert.Throws<ArgumentOutOfRangeException>(() => array.RemoveRange(transaction, index: 5, length: 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => array.RemoveRange(transaction, index: 1, length: 5));
        Assert.Throws<ArgumentOutOfRangeException>(() => array.RemoveRange(transaction, index: 1, length: uint.MaxValue));
        Assert.That(array.Length(transaction), Is.EqualTo(expected: 2));
    }

    [Test]
    public void ArrayRemoveRangeUpToEndSucceeds()
    {
        var doc = new Doc();
        var array = doc.Array("array");

        using var transaction = doc.WriteTransaction();

        array.InsertRange(transaction, index: 0, Input.Long(value: 1), Input.Long(value: 2));
        array.RemoveRange(transaction, index: 1, length: 1);

        Assert.That(array.Length(transaction), Is.EqualTo(expected: 1));
    }

    [Test]
    public void TextRangesPastEndThrow()
    {
        var doc = new Doc();
        var text = doc.Text("text");

        using var transaction = doc.WriteTransaction();

        text.Insert(transaction, index: 0, "ab");

        Assert.Throws<ArgumentOutOfRangeException>(() => text.RemoveRange(transaction, index: 1, length: 10));
        Assert.Throws<ArgumentOutOfRangeException>(() => text.RemoveRange(transaction, index: 5, length: 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => text.Format(transaction, index: 5, length: 1, Attributes()));
        Assert.That(text.Length(transaction), Is.EqualTo(expected: 2));
    }

    [Test]
    public void TextRangesUpToEndSucceed()
    {
        var doc = new Doc();
        var text = doc.Text("text");

        using var transaction = doc.WriteTransaction();

        text.Insert(transaction, index: 0, "ab");
        text.Format(transaction, index: 0, length: 2, Attributes());
        text.RemoveRange(transaction, index: 1, length: 1);

        Assert.That(text.String(transaction), Is.EqualTo("a"));
    }

    [Test]
    public void XmlTextRangesPastEndThrow()
    {
        var doc = new Doc();
        var fragment = doc.XmlFragment("fragment");

        using var transaction = doc.WriteTransaction();

        var text = fragment.InsertText(transaction, index: 0);
        text.Insert(transaction, index: 0, "ab");

        Assert.Throws<ArgumentOutOfRangeException>(() => text.RemoveRange(transaction, index: 1, length: 10));
        Assert.Throws<ArgumentOutOfRangeException>(() => text.Format(transaction, index: 5, length: 1, Attributes()));
        Assert.That(text.Length(transaction), Is.EqualTo(expected: 2));
    }

    [Test]
    public void XmlFragmentChildrenPastEndThrow()
    {
        var doc = new Doc();
        var fragment = doc.XmlFragment("fragment");

        using var transaction = doc.WriteTransaction();

        Assert.Throws<ArgumentOutOfRangeException>(() => fragment.InsertElement(transaction, index: 1, "p"));
        Assert.Throws<ArgumentOutOfRangeException>(() => fragment.InsertText(transaction, index: 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => fragment.RemoveRange(transaction, index: 0, length: 1));
        Assert.That(fragment.ChildLength(transaction), Is.EqualTo(expected: 0));
    }

    [Test]
    public void XmlElementChildrenPastEndThrow()
    {
        var doc = new Doc();
        var fragment = doc.XmlFragment("fragment");

        using var transaction = doc.WriteTransaction();

        var element = fragment.InsertElement(transaction, index: 0, "div");
        element.InsertElement(transaction, index: 0, "p");

        Assert.Throws<ArgumentOutOfRangeException>(() => element.InsertElement(transaction, index: 5, "p"));
        Assert.Throws<ArgumentOutOfRangeException>(() => element.InsertText(transaction, index: 5));
        Assert.Throws<ArgumentOutOfRangeException>(() => element.RemoveRange(transaction, index: 0, length: 5));
        Assert.That(element.ChildLength(transaction), Is.EqualTo(expected: 1));
    }
}
