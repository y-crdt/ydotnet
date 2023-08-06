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
        transaction.Commit();

        // Assert
        Assert.That(array.Length, Is.EqualTo(expected: 3));
    }

    [Test]
    public void RemoveSingleItemRange()
    {
        // Arrange
        var (doc, array) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        array.RemoveRange(transaction, index: 1, length: 1);
        transaction.Commit();

        // Assert
        Assert.That(array.Length, Is.EqualTo(expected: 2));
    }

    [Test]
    public void RemoveMultiItemRange()
    {
        // Arrange
        var (doc, array) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        array.RemoveRange(transaction, index: 0, length: 3);
        transaction.Commit();

        // Assert
        Assert.That(array.Length, Is.EqualTo(expected: 0));
    }

    private (Doc, Array) ArrangeDoc()
    {
        var doc = new Doc();
        var array = doc.Array("array");

        var transaction = doc.WriteTransaction();
        array.InsertRange(
            transaction, index: 0, new[]
            {
                Input.Long(value: 420L),
                Input.Boolean(value: true),
                Input.Null()
            });
        transaction.Commit();

        return (doc, array);
    }
}
