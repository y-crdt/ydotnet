using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;
using Array = YDotNet.Document.Types.Arrays.Array;

namespace YDotNet.Tests.Unit.Arrays;

public class RemoveRangeTests
{
    [Test]
    public void RemoveEmptyRange()
    {
        // Arrange
        var (doc, array) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        array.RemoveRange(transaction, index: 0, length: 0);
        var length = array.Length(transaction);
        transaction.Commit();

        // Assert
        Assert.That(length, Is.EqualTo(expected: 3));
    }

    [Test]
    public void RemoveSingleItemRange()
    {
        // Arrange
        var (doc, array) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        array.RemoveRange(transaction, index: 1, length: 1);
        var length = array.Length(transaction);
        transaction.Commit();

        // Assert
        Assert.That(length, Is.EqualTo(expected: 2));
    }

    [Test]
    public void RemoveMultiItemRange()
    {
        // Arrange
        var (doc, array) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        array.RemoveRange(transaction, index: 0, length: 3);
        var length = array.Length(transaction);
        transaction.Commit();

        // Assert
        Assert.That(length, Is.EqualTo(expected: 0));
    }

    private (Doc, Array) ArrangeDoc()
    {
        var doc = new Doc();
        var array = doc.Array("array");

        var transaction = doc.WriteTransaction();
        array.InsertRange(
            transaction, index: 0, Input.Long(value: 420L), Input.Boolean(value: true), Input.Null());
        transaction.Commit();

        return (doc, array);
    }
}
