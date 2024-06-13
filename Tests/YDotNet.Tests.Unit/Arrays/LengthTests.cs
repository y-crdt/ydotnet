using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;

namespace YDotNet.Tests.Unit.Arrays;

public class LengthTests
{
    [Test]
    public void InitialLengthIsZero()
    {
        // Arrange
        var doc = new Doc();
        var array = doc.Array("array");

        // Act
        var transaction = doc.ReadTransaction();
        var length = array.Length(transaction);
        transaction.Commit();

        // Assert
        Assert.That(length, Is.EqualTo(expected: 0));
    }

    [Test]
    public void IncreasesWhenAdded()
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

        // Act
        transaction = doc.WriteTransaction();
        array.InsertRange(
            transaction, index: 0, Input.Boolean(value: true));
        length = array.Length(transaction);
        transaction.Commit();

        // Assert
        Assert.That(length, Is.EqualTo(expected: 2));
    }

    [Test]
    public void DecreasesWhenRemoved()
    {
        // Arrange
        var doc = new Doc();
        var array = doc.Array("array");
        var transaction = doc.WriteTransaction();
        array.InsertRange(
            transaction, index: 0, Input.Boolean(value: true), Input.Long(value: 2469L), Input.Undefined());
        var length = array.Length(transaction);
        transaction.Commit();

        // Assert
        Assert.That(length, Is.EqualTo(expected: 3));

        // Act
        transaction = doc.WriteTransaction();
        array.RemoveRange(transaction, index: 1, length: 2);
        length = array.Length(transaction);
        transaction.Commit();

        // Assert
        Assert.That(length, Is.EqualTo(expected: 1));
    }
}
