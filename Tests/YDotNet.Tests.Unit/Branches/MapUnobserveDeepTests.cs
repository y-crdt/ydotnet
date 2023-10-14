using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;

namespace YDotNet.Tests.Unit.Branches;

public class MapUnobserveDeepTests
{
    [Test]
    public void TriggersWhenMapChangedUntilUnobserved()
    {
        // Arrange
        var doc = new Doc();
        var map = doc.Map("map");
        var called = 0;
        var subscription = map.ObserveDeep(_ => called++);

        // Act
        var transaction = doc.WriteTransaction();
        map.Insert(transaction, "value1", Input.Long(value: 2469L));
        transaction.Commit();

        // Assert
        Assert.That(called, Is.EqualTo(expected: 1));

        // Act
        subscription.Dispose();

        transaction = doc.WriteTransaction();
        map.Insert(transaction, "value2", Input.Long(value: -420L));
        transaction.Commit();

        // Assert
        Assert.That(called, Is.EqualTo(expected: 1));
    }
}
