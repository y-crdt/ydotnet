using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.StickyIndexes;

namespace YDotNet.Tests.Unit.StickyIndexes;

public class ReadTests
{
    [Test]
    public void ReadIndexFromText()
    {
        // Arrange
        var doc = new Doc();
        var text = doc.Text("text");

        var transaction = doc.WriteTransaction();
        text.Insert(transaction, index: 0, "Lucas");
        var stickyIndexBefore = text.StickyIndex(transaction, index: 3, StickyAssociationType.Before);
        var stickyIndexAfter = text.StickyIndex(transaction, index: 3, StickyAssociationType.After);
        transaction.Commit();

        // Act
        transaction = doc.ReadTransaction();
        var beforeIndex = stickyIndexBefore.Read(transaction);
        var afterIndex = stickyIndexAfter.Read(transaction);
        transaction.Commit();

        // Assert
        Assert.That(beforeIndex, Is.EqualTo(expected: 3));
        Assert.That(afterIndex, Is.EqualTo(expected: 3));

        // Act
        transaction = doc.WriteTransaction();
        text.Insert(transaction, index: 3, "(");
        text.Insert(transaction, index: 5, ")");
        text.Insert(transaction, index: 7, " Viana");
        text.Insert(transaction, index: 0, "Hello, ");
        beforeIndex = stickyIndexBefore.Read(transaction);
        afterIndex = stickyIndexAfter.Read(transaction);
        transaction.Commit();

        // Assert
        Assert.That(beforeIndex, Is.EqualTo(expected: 10));
        Assert.That(afterIndex, Is.EqualTo(expected: 11));
    }

    [Test]
    [Ignore("Waiting to be implemented.")]
    public void ReadIndexFromArray()
    {
    }

    [Test]
    [Ignore("Waiting to be implemented.")]
    public void ReadIndexFromMap()
    {
    }

    [Test]
    [Ignore("Waiting to be implemented.")]
    public void ReadIndexFromXmlText()
    {
    }

    [Test]
    [Ignore("Waiting to be implemented.")]
    public void ReadIndexFromXmlElement()
    {
    }
}
