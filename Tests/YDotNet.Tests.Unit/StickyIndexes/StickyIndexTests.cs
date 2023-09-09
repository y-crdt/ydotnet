using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.StickyIndexes;

namespace YDotNet.Tests.Unit.StickyIndexes;

public class StickyIndexTests
{
    [Test]
    public void CreateFromEmptyText()
    {
        // Arrange
        var doc = new Doc();
        var text = doc.Text("text");

        // Act
        var transaction = doc.WriteTransaction();
        var stickyIndexAfter = text.StickyIndex(transaction, index: 0, StickyAssociationType.After);
        var stickyIndexBefore = text.StickyIndex(transaction, index: 0, StickyAssociationType.Before);
        transaction.Commit();

        // Assert
        Assert.That(stickyIndexAfter, Is.Null);
        Assert.That(stickyIndexBefore, Is.Not.Null);
        Assert.That(stickyIndexBefore.Handle, Is.GreaterThan(nint.Zero));
    }

    [Test]
    [Ignore("Waiting to be implemented.")]
    public void CreateFromFilledTextAtBeginning()
    {
    }

    [Test]
    [Ignore("Waiting to be implemented.")]
    public void CreateFromFilledTextAtMiddle()
    {
    }

    [Test]
    [Ignore("Waiting to be implemented.")]
    public void CreateFromFilledTextAtEnding()
    {
    }

    [Test]
    [Ignore("Waiting to be implemented.")]
    public void CreateFromEmptyArray()
    {
    }

    [Test]
    [Ignore("Waiting to be implemented.")]
    public void CreateFromFilledArrayAtBeginning()
    {
    }

    [Test]
    [Ignore("Waiting to be implemented.")]
    public void CreateFromFilledArrayAtMiddle()
    {
    }

    [Test]
    [Ignore("Waiting to be implemented.")]
    public void CreateFromFilledArrayAtEnding()
    {
    }

    [Test]
    [Ignore("Waiting to be implemented.")]
    public void CreateFromEmptyMap()
    {
    }

    [Test]
    [Ignore("Waiting to be implemented.")]
    public void CreateFromFilledMap()
    {
    }

    [Test]
    [Ignore("Waiting to be implemented.")]
    public void CreateFromEmptyXmlText()
    {
    }

    [Test]
    [Ignore("Waiting to be implemented.")]
    public void CreateFromFilledXmlText()
    {
    }

    [Test]
    [Ignore("Waiting to be implemented.")]
    public void CreateFromEmptyXmlElement()
    {
    }

    [Test]
    [Ignore("Waiting to be implemented.")]
    public void CreateFromFilledXmlElement()
    {
    }
}
