using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.StickyIndexes;

namespace YDotNet.Tests.Unit.StickyIndexes;

public class StickyIndexTests
{
    [Test]
    public void CreateFromText()
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
    public void CreateFromArray()
    {
    }

    [Test]
    [Ignore("Waiting to be implemented.")]
    public void CreateFromMap()
    {
    }

    [Test]
    [Ignore("Waiting to be implemented.")]
    public void CreateFromdXmlText()
    {
    }

    [Test]
    [Ignore("Waiting to be implemented.")]
    public void CreateFromXmlElement()
    {
    }
}
