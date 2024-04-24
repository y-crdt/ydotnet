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
        array.InsertRange(transaction, index: 0);
        var length = array.Length(transaction);
        transaction.Commit();

        // Assert
        Assert.That(length, Is.EqualTo(expected: 0));
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
            transaction, index: 0, Input.Boolean(value: true));
        var length = array.Length(transaction);
        transaction.Commit();

        // Assert
        Assert.That(length, Is.EqualTo(expected: 1));
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
            transaction, index: 0, Input.Boolean(value: true), Input.Long(value: 2469L));
        var length = array.Length(transaction);
        transaction.Commit();

        // Assert
        Assert.That(length, Is.EqualTo(expected: 2));
    }
}
