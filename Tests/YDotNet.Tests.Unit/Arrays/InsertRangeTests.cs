using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;

namespace YDotNet.Tests.Unit.Arrays;

public class InsertRangeTests
{
    [Test]
    public void InsertEmptyRange()
    {
        // Arrange
        var doc = new Doc();
        var array = doc.Array("array");

        // Act
        var transaction = doc.WriteTransaction();
        array.InsertRange(transaction, index: 0, Enumerable.Empty<Input>());
        transaction.Commit();

        // Assert
        Assert.That(array.Length, Is.EqualTo(expected: 0));
    }

    [Test]
    public void InsertSingleItemRange()
    {
        // Arrange
        var doc = new Doc();
        var array = doc.Array("array");

        // Act
        var transaction = doc.WriteTransaction();
        array.InsertRange(
            transaction, index: 0, new[]
            {
                Input.Boolean(value: true)
            });
        transaction.Commit();

        // Assert
        Assert.That(array.Length, Is.EqualTo(expected: 1));
    }

    [Test]
    public void InsertMultiItemRange()
    {
        // Arrange
        var doc = new Doc();
        var array = doc.Array("array");

        // Act
        var transaction = doc.WriteTransaction();
        array.InsertRange(
            transaction, index: 0, new[]
            {
                Input.Boolean(value: true),
                Input.Long(value: 2469L)
            });
        transaction.Commit();

        // Assert
        Assert.That(array.Length, Is.EqualTo(expected: 2));
    }
}
