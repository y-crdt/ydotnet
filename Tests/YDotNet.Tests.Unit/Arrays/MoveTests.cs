using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;
using Array = YDotNet.Document.Types.Arrays.Array;

namespace YDotNet.Tests.Unit.Arrays;

public class MoveTests
{
    [Test]
    public void MoveFromBeginningToEnding()
    {
        // Arrange
        var (doc, array) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        array.Move(transaction, sourceIndex: 0, array.Length);
        transaction.Commit();

        // Assert
        AssertValues(doc, array, 2, 3, 4, 1);
    }

    [Test]
    public void MoveFromEndingToBeginning()
    {
        // Arrange
        var (doc, array) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        array.Move(transaction, array.Length - 1, targetIndex: 0);
        transaction.Commit();

        // Assert
        AssertValues(doc, array, 4, 1, 2, 3);
    }

    [Test]
    public void MoveInTheMiddle()
    {
        // Arrange
        var (doc, array) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        array.Move(transaction, sourceIndex: 2, targetIndex: 1);
        transaction.Commit();

        // Assert
        AssertValues(doc, array, 1, 3, 2, 4);
    }

    [Test]
    public void MoveToTheSameIndex()
    {
        // Arrange
        var (doc, array) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        array.Move(transaction, sourceIndex: 2, targetIndex: 2);
        transaction.Commit();

        // Assert
        AssertValues(doc, array, 1, 2, 3, 4);
    }

    private (Doc, Array) ArrangeDoc()
    {
        var doc = new Doc();
        var array = doc.Array("array");

        var transaction = doc.WriteTransaction();
        array.InsertRange(
            transaction, index: 0, new[]
            {
                Input.Long(value: 1),
                Input.Long(value: 2),
                Input.Long(value: 3),
                Input.Long(value: 4)
            });
        transaction.Commit();

        return (doc, array);
    }

    private void AssertValues(Doc doc, Array array, params int[] values)
    {
        var transaction = doc.ReadTransaction();

        for (var index = 0u; index < values.Length; index++)
        {
            Assert.That(array.Get(transaction, index).Long, Is.EqualTo(values[index]));
        }

        transaction.Commit();
    }
}
