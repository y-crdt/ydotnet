using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;
using YDotNet.Document.Transactions;
using YDotNet.Document.Types.Maps;

namespace YDotNet.Tests.Unit.Maps;

public class LengthTests
{
    [Test]
    public void InitialLengthIsZero()
    {
        // Arrange and Act
        var (map, transaction) = ArrangeMap();

        // Assert
        Assert.That(map.Length(transaction), Is.EqualTo(expected: 0));
    }

    [Test]
    public void IncreasesWhenAdded()
    {
        // Arrange
        var (map, transaction) = ArrangeMap();

        // Act
        map.Insert(transaction, "value1", Input.Long(value: 2469L));

        // Assert
        Assert.That(map.Length(transaction), Is.EqualTo(expected: 1));

        // Act
        map.Insert(transaction, "value2", Input.Long(value: -420L));

        // Assert
        Assert.That(map.Length(transaction), Is.EqualTo(expected: 2));
    }

    [Test]
    public void DecreasesWhenRemoved()
    {
        // Arrange and Act
        var (map, transaction) = ArrangeMap();

        map.Insert(transaction, "value1", Input.Long(value: 2469L));
        map.Insert(transaction, "value2", Input.Long(value: -420L));

        // Assert
        Assert.That(map.Length(transaction), Is.EqualTo(expected: 2));

        // Act
        var result = map.Remove(transaction, "value1");

        // Assert
        Assert.That(result, Is.True);
        Assert.That(map.Length(transaction), Is.EqualTo(expected: 1));
    }

    private (Map, Transaction) ArrangeMap()
    {
        var doc = new Doc();
        var map = doc.Map("map");
        var transaction = doc.WriteTransaction();

        return (map, transaction);
    }
}
