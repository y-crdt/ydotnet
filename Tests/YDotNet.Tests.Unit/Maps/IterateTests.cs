using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;
using YDotNet.Document.Transactions;
using YDotNet.Document.Types.Maps;

namespace YDotNet.Tests.Unit.Maps;

public class IterateTests
{
    [Test]
    public void IteratesOnEmptyMaps()
    {
        // Arrange
        var (map, transaction) = Arrange();

        // Act
        var iterator = map.Iterate(transaction);

        // Assert
        Assert.That(iterator, Is.Empty);
    }

    [Test]
    public void IteratesOnMapsWithASingleEntry()
    {
        // Arrange
        var (map, transaction) = Arrange();
        map.Insert(transaction, "value1", Input.Long(value: 2469L));

        // Act
        var iterator = map.Iterate(transaction);

        // Assert
        var items = iterator.ToArray();
        Assert.That(items.Length, Is.EqualTo(expected: 1));
        Assert.That(items.ElementAt(index: 0).Key, Is.EqualTo("value1"));
        Assert.That(items.ElementAt(index: 0).Value.Long, Is.EqualTo(expected: 2469L));
    }

    [Test]
    public void IteratesOnMapsWithMultipleEntries()
    {
        // Arrange
        var (map, transaction) = Arrange();
        map.Insert(transaction, "value-1️⃣", Input.Long(value: 2469L));
        map.Insert(transaction, "value-2️⃣", Input.Long(value: -420L));

        // Act
        var iterator = map.Iterate(transaction);

        // Assert
        var items = iterator.OrderBy(x => x.Key).ToArray();
        Assert.That(items.Length, Is.EqualTo(expected: 2));
        Assert.That(items.ElementAt(index: 0).Key, Is.EqualTo("value-1️"));
        Assert.That(items.ElementAt(index: 0).Value.Long, Is.EqualTo(expected: 2469L));
        Assert.That(items.ElementAt(index: 1).Key, Is.EqualTo("value-2️⃣"));
        Assert.That(items.ElementAt(index: 1).Value.Long, Is.EqualTo(expected: -420L));
    }

    private (Map, Transaction) Arrange()
    {
        var doc = new Doc();
        var map = doc.Map("map");
        var transaction = doc.WriteTransaction();

        return (map, transaction);
    }
}
