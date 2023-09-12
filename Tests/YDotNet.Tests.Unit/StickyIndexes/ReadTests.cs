using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;
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
    public void ReadIndexFromArray()
    {
        // Arrange
        var doc = new Doc();
        var array = doc.Array("array");

        var transaction = doc.WriteTransaction();
        array.InsertRange(
            transaction, index: 0, new[] { Input.Long(value: 2469L), Input.Null(), Input.Boolean(value: false) });
        var stickyIndexBefore = array.StickyIndex(transaction, index: 1, StickyAssociationType.Before);
        var stickyIndexAfter = array.StickyIndex(transaction, index: 1, StickyAssociationType.After);
        transaction.Commit();

        // Act
        transaction = doc.ReadTransaction();
        var beforeIndex = stickyIndexBefore.Read(transaction);
        var afterIndex = stickyIndexAfter.Read(transaction);
        transaction.Commit();

        // Assert
        Assert.That(beforeIndex, Is.EqualTo(expected: 1));
        Assert.That(afterIndex, Is.EqualTo(expected: 1));

        // Act
        transaction = doc.WriteTransaction();
        array.InsertRange(transaction, index: 1, new[] { Input.String("(") });
        array.InsertRange(transaction, index: 3, new[] { Input.String(")") });
        array.InsertRange(transaction, index: 4, new[] { Input.String(" Viana") });
        array.InsertRange(transaction, index: 0, new[] { Input.String("Hello, ") });
        beforeIndex = stickyIndexBefore.Read(transaction);
        afterIndex = stickyIndexAfter.Read(transaction);
        transaction.Commit();

        // Assert
        Assert.That(beforeIndex, Is.EqualTo(expected: 2));
        Assert.That(afterIndex, Is.EqualTo(expected: 3));
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
