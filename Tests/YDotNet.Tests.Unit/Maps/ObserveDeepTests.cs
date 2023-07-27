using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;
using YDotNet.Document.Events;
using YDotNet.Document.Types;
using YDotNet.Document.Types.Events;

namespace YDotNet.Tests.Unit.Maps;

public class ObserveDeepTests
{
    [Test]
    public void ObserveHasPathWhenAdded()
    {
        // Arrange
        var (doc, map1, _, map3) = ArrangeDoc();

        IEnumerable<EventPathSegment>? pathSegments = null;
        var called = 0;

        var subscription = map1.ObserveDeep(
            events =>
            {
                called++;
                pathSegments = events.First().MapEvent.Path.ToArray();
            });

        // Act
        var transaction = doc.WriteTransaction();
        map3.Insert(transaction, "value", Input.Long(value: 2469L));
        transaction.Commit();

        // Assert
        AssertPath(called, subscription, pathSegments, "map-2", "map-3");
    }

    [Test]
    public void ObserveHasPathWhenRemovedByKey()
    {
        // Arrange
        var (doc, map1, _, map3) = ArrangeDoc();

        var transaction = doc.WriteTransaction();
        map3.Insert(transaction, "value", Input.Long(value: 2469L));
        transaction.Commit();

        IEnumerable<EventPathSegment>? pathSegments = null;
        var called = 0;

        var subscription = map1.ObserveDeep(
            events =>
            {
                called++;
                pathSegments = events.First().MapEvent.Path.ToArray();
            });

        // Act
        transaction = doc.WriteTransaction();
        map3.Remove(transaction, "value");
        transaction.Commit();

        // Assert
        AssertPath(called, subscription, pathSegments, "map-2", "map-3");
    }

    [Test]
    public void ObserveHasPathWhenRemovedAll()
    {
        // Arrange
        var (doc, map1, _, map3) = ArrangeDoc();

        var transaction = doc.WriteTransaction();
        map3.Insert(transaction, "value", Input.Long(value: 2469L));
        transaction.Commit();

        IEnumerable<EventPathSegment>? pathSegments = null;
        var called = 0;

        var subscription = map1.ObserveDeep(
            events =>
            {
                called++;
                pathSegments = events.First().MapEvent.Path.ToArray();
            });

        // Act
        transaction = doc.WriteTransaction();
        map3.RemoveAll(transaction);
        transaction.Commit();

        // Assert
        AssertPath(called, subscription, pathSegments, "map-2", "map-3");
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

    private static (Doc, Map, Map, Map) ArrangeDoc()
    {
        var doc = new Doc();
        var map1 = doc.Map("map-1");

        var transaction = doc.WriteTransaction();
        map1.Insert(transaction, "map-2", Input.Map(new Dictionary<string, Input>()));

        var map2 = map1.Get(transaction, "map-2").Map;
        map2.Insert(transaction, "map-3", Input.Map(new Dictionary<string, Input>()));

        var map3 = map2.Get(transaction, "map-3").Map;
        transaction.Commit();

        return (doc, map1, map2, map3);
    }

    private static void AssertPath(
        int called,
        EventSubscription subscription,
        IEnumerable<EventPathSegment>? pathSegments,
        params string[] path)
    {
        Assert.That(called, Is.EqualTo(expected: 1));
        Assert.That(subscription.Id, Is.EqualTo(expected: 0L));
        Assert.That(pathSegments, Is.Not.Null);
        Assert.That(pathSegments.Count(), Is.EqualTo(path.Length));

        for (var i = 0; i < path.Length; i++)
        {
            var segment = pathSegments.ElementAt(i);

            Assert.That(segment.Tag, Is.EqualTo(EventPathSegmentTag.Key));
            Assert.That(segment.Key, Is.EqualTo(path[i]));
            Assert.That(segment.Index, Is.Null);
        }
    }
}
