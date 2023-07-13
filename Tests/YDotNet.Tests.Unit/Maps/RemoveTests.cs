using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;
using YDotNet.Document.Transactions;
using YDotNet.Document.Types;

namespace YDotNet.Tests.Unit.Maps;

public class RemoveTests
{
    [Test]
    public void ReturnsTrueWhenRemovingValidItems()
    {
        // Arrange
        var (map, transaction) = ArrangeMap();

        // Act
        var result1 = map.Remove(transaction, "value1");
        var result2 = map.Remove(transaction, "value2");

        // Assert
        Assert.That(result1, Is.True);
        Assert.That(result2, Is.True);
    }

    [Test]
    public void ReturnsFalseWhenRemovingInvalidItems()
    {
        // Arrange
        var (map, transaction) = ArrangeMap();

        // Act
        var result1 = map.Remove(transaction, "");
        var result2 = map.Remove(transaction, "xxx");

        // Assert
        Assert.That(result1, Is.False);
        Assert.That(result2, Is.False);
    }

    private (Map, Transaction) ArrangeMap()
    {
        var doc = new Doc();
        var map = doc.Map("map");
        var transaction = doc.WriteTransaction();

        map.Insert(transaction, "value1", Input.Long(value: 2469L));
        map.Insert(transaction, "value2", Input.Long(value: -420L));

        return (map, transaction);
    }
}
