using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;
using YDotNet.Document.Types.Maps.Events;

namespace YDotNet.Tests.Unit.Maps;

public class ObserveTests
{
    [Test]
    public void ObserveHasKeysWhenAdded()
    {
        // Arrange
        var doc = new Doc();
        var map = doc.Map("map");

        IEnumerable<MapEventKeyChange>? keyChanges = null;
        var called = 0;

        var subscription = map.Observe(
            e =>
            {
                called++;
                keyChanges = e.Keys.ToArray();
            });

        // Act
        var transaction = doc.WriteTransaction();
        map.Insert(transaction, "value", Input.Long(value: 2469L));
        transaction.Commit();

        // Assert
        var firstKey = keyChanges.First();

        Assert.That(called, Is.EqualTo(expected: 1));
        Assert.That(subscription.Id, Is.EqualTo(expected: 0L));
        Assert.That(keyChanges, Is.Not.Null);
        Assert.That(keyChanges.Count(), Is.EqualTo(expected: 1));
        Assert.That(firstKey.Key, Is.EqualTo("value"));
        Assert.That(firstKey.Tag, Is.EqualTo(MapEventKeyChangeTag.Add));
        Assert.That(firstKey.OldValue, Is.Null);
        Assert.That(firstKey.NewValue, Is.Not.Null);
        Assert.That(firstKey.NewValue.Long, Is.EqualTo(expected: 2469L));
        Assert.That(firstKey.NewValue.Double, Is.Null);
    }

    [Test]
    public void ObserveHasKeysWhenRemovedByKey()
    {
        // Arrange
        var doc = new Doc();
        var map = doc.Map("map");
        var transaction = doc.WriteTransaction();
        map.Insert(transaction, "value", Input.Long(value: 2469L));
        transaction.Commit();

        IEnumerable<MapEventKeyChange>? keyChanges = null;
        var called = 0;

        var subscription = map.Observe(
            e =>
            {
                called++;
                keyChanges = e.Keys.ToArray();
            });

        // Act
        transaction = doc.WriteTransaction();
        var removed = map.Remove(transaction, "value");
        transaction.Commit();

        // Assert
        var firstKey = keyChanges.First();

        Assert.That(removed, Is.True);
        Assert.That(called, Is.EqualTo(expected: 1));
        Assert.That(subscription.Id, Is.EqualTo(expected: 0L));
        Assert.That(keyChanges, Is.Not.Null);
        Assert.That(keyChanges.Count(), Is.EqualTo(expected: 1));
        Assert.That(firstKey.Key, Is.EqualTo("value"));
        Assert.That(firstKey.Tag, Is.EqualTo(MapEventKeyChangeTag.Remove));
        Assert.That(firstKey.OldValue, Is.Not.Null);
        Assert.That(firstKey.NewValue, Is.Null);
        Assert.That(firstKey.OldValue.Long, Is.EqualTo(expected: 2469L));
        Assert.That(firstKey.OldValue.Double, Is.Null);
    }

    [Test]
    public void ObserveHasKeysWhenRemovedAll()
    {
        // Arrange
        var doc = new Doc();
        var map = doc.Map("map");
        var transaction = doc.WriteTransaction();
        map.Insert(transaction, "value", Input.Long(value: 2469L));
        transaction.Commit();

        IEnumerable<MapEventKeyChange>? keyChanges = null;
        var called = 0;

        var subscription = map.Observe(
            e =>
            {
                called++;
                keyChanges = e.Keys.ToArray();
            });

        // Act
        transaction = doc.WriteTransaction();
        map.RemoveAll(transaction);
        transaction.Commit();

        // Assert
        var firstKey = keyChanges.First();

        Assert.That(called, Is.EqualTo(expected: 1));
        Assert.That(subscription.Id, Is.EqualTo(expected: 0L));
        Assert.That(keyChanges, Is.Not.Null);
        Assert.That(keyChanges.Count(), Is.EqualTo(expected: 1));
        Assert.That(firstKey.Key, Is.EqualTo("value"));
        Assert.That(firstKey.Tag, Is.EqualTo(MapEventKeyChangeTag.Remove));
        Assert.That(firstKey.OldValue, Is.Not.Null);
        Assert.That(firstKey.NewValue, Is.Null);
        Assert.That(firstKey.OldValue.Long, Is.EqualTo(expected: 2469L));
        Assert.That(firstKey.OldValue.Double, Is.Null);
    }

    [Test]
    public void ObserveHasKeysWhenUpdated()
    {
        // Arrange
        var doc = new Doc();
        var map = doc.Map("map");
        var transaction = doc.WriteTransaction();
        map.Insert(transaction, "value", Input.Long(value: 2469L));
        transaction.Commit();

        IEnumerable<MapEventKeyChange>? keyChanges = null;
        var called = 0;

        var subscription = map.Observe(
            e =>
            {
                called++;
                keyChanges = e.Keys.ToArray();
            });

        // Act
        transaction = doc.WriteTransaction();
        map.Insert(transaction, "value", Input.Long(value: -420L));
        transaction.Commit();

        // Assert
        var firstKey = keyChanges.First();

        Assert.That(called, Is.EqualTo(expected: 1));
        Assert.That(subscription.Id, Is.EqualTo(expected: 0L));
        Assert.That(keyChanges, Is.Not.Null);
        Assert.That(keyChanges.Count(), Is.EqualTo(expected: 1));
        Assert.That(firstKey.Key, Is.EqualTo("value"));
        Assert.That(firstKey.Tag, Is.EqualTo(MapEventKeyChangeTag.Update));
        Assert.That(firstKey.OldValue, Is.Not.Null);
        Assert.That(firstKey.NewValue, Is.Not.Null);
        Assert.That(firstKey.OldValue.Long, Is.EqualTo(expected: 2469L));
        Assert.That(firstKey.OldValue.Double, Is.Null);
        Assert.That(firstKey.NewValue.Long, Is.EqualTo(expected: -420L));
        Assert.That(firstKey.NewValue.Double, Is.Null);
    }

    [Test]
    [Ignore("To be implemented.")]
    public void ObserveHasKeysWhenAddedAndRemovedAndUpdated()
    {
    }

    [Test]
    [Ignore("To be implemented.")]
    public void ObserveHasPathWhenAdded()
    {
    }

    [Test]
    [Ignore("To be implemented.")]
    public void ObserveHasPathWhenRemovedByKey()
    {
    }

    [Test]
    [Ignore("To be implemented.")]
    public void ObserveHasPathWhenRemovedAll()
    {
    }

    [Test]
    [Ignore("To be implemented.")]
    public void ObserveHasPathWhenUpdated()
    {
    }

    [Test]
    [Ignore("To be implemented.")]
    public void ObserveHasPathWhenAddedAndRemovedAndUpdated()
    {
    }
}
