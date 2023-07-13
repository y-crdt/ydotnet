using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;
using YDotNet.Document.Transactions;
using YDotNet.Document.Types;

namespace YDotNet.Tests.Unit.Maps;

public class RemoveAllTests
{
    [Test]
    public void RemovesAllValues()
    {
        // Arrange
        var (map, transaction) = ArrangeMap();

        // Assert
        Assert.That(map.Length(transaction), Is.EqualTo(expected: 2));

        // Act
        map.RemoveAll(transaction);

        // Assert
        Assert.That(map.Length(transaction), Is.EqualTo(expected: 0));
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
