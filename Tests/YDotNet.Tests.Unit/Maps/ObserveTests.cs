using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;
using YDotNet.Document.Types.Events;
using YDotNet.Document.Types.Maps;

namespace YDotNet.Tests.Unit.Maps;

public class ObserveTests
{
    [Test]
    public void ObserveHasTarget()
    {
        // Arrange
        var doc = new Doc();
        var map = doc.Map("map");
        Map? target = null;
        map.Observe(e => target = e.Target);

        // Act
        var transaction = doc.WriteTransaction();
        map.Insert(transaction, "value", Input.Long(value: 2469L));
        transaction.Commit();

        // Assert
        Assert.That(target, Is.Not.Null);
        Assert.That(target.Handle, Is.Not.EqualTo(nint.Zero));
    }

    [Test]
    public void ObserveHasKeysWhenAdded()
    {
        // Arrange
        var doc = new Doc();
        var map = doc.Map("map");

        IEnumerable<EventKeyChange>? keyChanges = null;
        var called = 0;

        var subscription = map.Observe(
            e =>
            {
                called++;
                keyChanges = e.Keys.ToArray();
            });

        // Act
        var transaction = doc.WriteTransaction();
        map.Insert(transaction, "moon-🌕", Input.Long(value: 2469L));
        transaction.Commit();

        // Assert
        var firstKey = keyChanges.First();

        Assert.That(called, Is.EqualTo(expected: 1));
        Assert.That(keyChanges, Is.Not.Null);
        Assert.That(keyChanges.Count(), Is.EqualTo(expected: 1));
        Assert.That(firstKey.Key, Is.EqualTo("moon-🌕"));
        Assert.That(firstKey.Tag, Is.EqualTo(EventKeyChangeTag.Add));
        Assert.That(firstKey.OldValue, Is.Null);
        Assert.That(firstKey.NewValue, Is.Not.Null);
        Assert.That(firstKey.NewValue.Long, Is.EqualTo(expected: 2469L));
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

        IEnumerable<EventKeyChange>? keyChanges = null;
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
        Assert.That(keyChanges, Is.Not.Null);
        Assert.That(keyChanges.Count(), Is.EqualTo(expected: 1));
        Assert.That(firstKey.Key, Is.EqualTo("value"));
        Assert.That(firstKey.Tag, Is.EqualTo(EventKeyChangeTag.Remove));
        Assert.That(firstKey.OldValue, Is.Not.Null);
        Assert.That(firstKey.NewValue, Is.Null);
        Assert.That(firstKey.OldValue.Long, Is.EqualTo(expected: 2469L));
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

        IEnumerable<EventKeyChange>? keyChanges = null;
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
        Assert.That(keyChanges, Is.Not.Null);
        Assert.That(keyChanges.Count(), Is.EqualTo(expected: 1));
        Assert.That(firstKey.Key, Is.EqualTo("value"));
        Assert.That(firstKey.Tag, Is.EqualTo(EventKeyChangeTag.Remove));
        Assert.That(firstKey.OldValue, Is.Not.Null);
        Assert.That(firstKey.NewValue, Is.Null);
        Assert.That(firstKey.OldValue.Long, Is.EqualTo(expected: 2469L));
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

        IEnumerable<EventKeyChange>? keyChanges = null;
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
        Assert.That(keyChanges, Is.Not.Null);
        Assert.That(keyChanges.Count(), Is.EqualTo(expected: 1));
        Assert.That(firstKey.Key, Is.EqualTo("value"));
        Assert.That(firstKey.Tag, Is.EqualTo(EventKeyChangeTag.Update));
        Assert.That(firstKey.OldValue, Is.Not.Null);
        Assert.That(firstKey.NewValue, Is.Not.Null);
        Assert.That(firstKey.OldValue.Long, Is.EqualTo(expected: 2469L));
        Assert.That(firstKey.NewValue.Long, Is.EqualTo(expected: -420L));
    }

    [Test]
    public void ObserveHasKeysWhenAddedAndRemovedAndUpdated()
    {
        // Arrange
        var doc = new Doc();
        var map = doc.Map("map");
        var transaction = doc.WriteTransaction();
        map.Insert(transaction, "value1", Input.Long(value: 2469L));
        map.Insert(transaction, "value2", Input.Long(value: -420L));
        transaction.Commit();

        IEnumerable<EventKeyChange>? keyChanges = null;
        var called = 0;

        var subscription = map.Observe(
            e =>
            {
                called++;
                keyChanges = e.Keys.ToArray();
            });

        // Act
        transaction = doc.WriteTransaction();

        // Update, remove, and add, respectively
        map.Insert(transaction, "value1", Input.Long(value: -420L));
        map.Remove(transaction, "value2");
        map.Insert(transaction, "value3", Input.Long(value: -1337L));

        transaction.Commit();

        // Assert
        Assert.That(called, Is.EqualTo(expected: 1));
        Assert.That(keyChanges, Is.Not.Null);
        Assert.That(keyChanges.Count(), Is.EqualTo(expected: 3));
        Assert.That(keyChanges.Count(x => x.Tag == EventKeyChangeTag.Update), Is.EqualTo(expected: 1));
        Assert.That(keyChanges.Count(x => x.Tag == EventKeyChangeTag.Remove), Is.EqualTo(expected: 1));
        Assert.That(keyChanges.Count(x => x.Tag == EventKeyChangeTag.Add), Is.EqualTo(expected: 1));

        var update = keyChanges.First(x => x.Tag == EventKeyChangeTag.Update);
        Assert.That(update.OldValue, Is.Not.Null);
        Assert.That(update.NewValue, Is.Not.Null);
        Assert.That(update.OldValue.Long, Is.EqualTo(expected: 2469L));
        Assert.That(update.NewValue.Long, Is.EqualTo(expected: -420L));

        var remove = keyChanges.First(x => x.Tag == EventKeyChangeTag.Remove);
        Assert.That(remove.OldValue, Is.Not.Null);
        Assert.That(remove.NewValue, Is.Null);
        Assert.That(remove.OldValue.Long, Is.EqualTo(expected: -420L));

        var add = keyChanges.First(x => x.Tag == EventKeyChangeTag.Add);
        Assert.That(add.OldValue, Is.Null);
        Assert.That(add.NewValue, Is.Not.Null);
        Assert.That(add.NewValue.Long, Is.EqualTo(expected: -1337L));
    }
}
