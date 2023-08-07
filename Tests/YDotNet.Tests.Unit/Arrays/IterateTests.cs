using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;

namespace YDotNet.Tests.Unit.Arrays;

public class IterateTests
{
    [Test]
    public void IteratesOnEmpty()
    {
        // Arrange
        var doc = new Doc();
        var array = doc.Array("array");

        // Act
        var transaction = doc.ReadTransaction();
        var iterator = array.Iterate(transaction);
        var length = iterator.Count();
        transaction.Commit();

        // Assert
        Assert.That(length, Is.EqualTo(expected: 0));
    }

    [Test]
    public void IteratesOnSingleItem()
    {
        // Arrange
        var doc = new Doc();
        var array = doc.Array("array");

        var transaction = doc.WriteTransaction();
        array.InsertRange(
            transaction, index: 0, new[]
            {
                Input.Long(value: 2469L)
            });
        transaction.Commit();

        // Act
        transaction = doc.ReadTransaction();
        var iterator = array.Iterate(transaction);
        var length = iterator.Count();
        transaction.Commit();

        // Assert
        Assert.That(length, Is.EqualTo(expected: 1));
    }

    [Test]
    public void IteratesOnMultiItem()
    {
        // Arrange
        var doc = new Doc();
        var array = doc.Array("array");

        var transaction = doc.WriteTransaction();
        array.InsertRange(
            transaction, index: 0, new[]
            {
                Input.Long(value: 2469L),
                Input.Boolean(value: false),
                Input.Undefined()
            });
        transaction.Commit();

        // Act
        transaction = doc.ReadTransaction();
        var iterator = array.Iterate(transaction);
        var values = iterator.ToArray();
        transaction.Commit();

        // Assert
        Assert.That(values.Length, Is.EqualTo(expected: 3));
        Assert.That(values[0].Long, Is.EqualTo(expected: 2469L));
        Assert.That(values[0].Double, Is.Null);
        Assert.That(values[1].Boolean, Is.False);
        Assert.That(values[1].Double, Is.Null);
        Assert.That(values[2].Undefined, Is.True);
        Assert.That(values[2].Double, Is.Null);
    }
}
