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
        Assert.That(values[1].Boolean, Is.False);
        Assert.That(values[2].Tag, Is.EqualTo(OutputTag.Undefined));
    }

    [Test]
    public void ResetRestartsTheEnumeration()
    {
        // Arrange
        var doc = new Doc();
        var array = doc.Array("array");

        var setup = doc.WriteTransaction();
        array.InsertRange(setup, index: 0, Input.Long(value: 1L), Input.Long(value: 2L), Input.Long(value: 3L));
        setup.Commit();

        var transaction = doc.ReadTransaction();
        using var enumerator = array.Iterate(transaction).GetEnumerator();
        var firstPass = new List<long>();
        while (enumerator.MoveNext())
        {
            firstPass.Add(enumerator.Current.Long);
        }

        // Act
        enumerator.Reset();
        var secondPass = new List<long>();
        while (enumerator.MoveNext())
        {
            secondPass.Add(enumerator.Current.Long);
        }

        transaction.Commit();

        // Assert
        Assert.That(firstPass, Has.Count.EqualTo(expected: 3));
        Assert.That(secondPass, Is.EqualTo(firstPass));
    }

    [Test]
    public void ResetThrowsAfterTransactionIsCommitted()
    {
        // Arrange
        var doc = new Doc();
        var array = doc.Array("array");

        var setup = doc.WriteTransaction();
        array.InsertRange(setup, index: 0, Input.Long(value: 1L), Input.Long(value: 2L), Input.Long(value: 3L));
        setup.Commit();

        var transaction = doc.ReadTransaction();
        var enumerator = array.Iterate(transaction).GetEnumerator();
        transaction.Commit();

        // Act and Assert
        Assert.Throws<ObjectDisposedException>(() => enumerator.Reset());
    }

    [Test]
    public void ThrowsWhenTransactionIsCommitted()
    {
        // Arrange
        var doc = new Doc();
        var array = doc.Array("array");

        var transaction = doc.ReadTransaction();
        transaction.Commit();

        // Act and Assert
        Assert.Throws<ObjectDisposedException>(() => array.Iterate(transaction));
    }
}
