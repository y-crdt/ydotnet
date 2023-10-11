using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;
using YDotNet.Document.Types.Events;
using YDotNet.Document.Types.Maps;

namespace YDotNet.Tests.Unit.Branches;

public class MapObserveDeepTests
{
    [Test]
    public void ObserveDeepHasPathWhenAdded()
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
        AssertPath(called, pathSegments, "map-2", "map-3");
    }

    [Test]
    public void ObserveDeepHasPathWhenRemovedByKey()
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
        AssertPath(called, pathSegments, "map-2", "map-3");
    }

    [Test]
    public void ObserveDeepHasPathWhenRemovedAll()
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
        AssertPath(called, pathSegments, "map-2", "map-3");
    }

    [Test]
    public void ObserveDeepHasPathWhenUpdated()
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
        map3.Insert(transaction, "value", Input.Long(value: -420L));
        transaction.Commit();

        // Assert
        AssertPath(called, pathSegments, "map-2", "map-3");
    }

    [Test]
    public void ObserveDeepHasPathWhenAddedAndRemovedAndUpdated()
    {
        // Arrange
        var (doc, map1, _, map3) = ArrangeDoc();

        var transaction = doc.WriteTransaction();
        map3.Insert(transaction, "value1", Input.Long(value: 2469L));
        map3.Insert(transaction, "value2", Input.Long(value: -420L));
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

        // Update, remove, and add, respectively
        map3.Insert(transaction, "value1", Input.Long(value: -420L));
        map3.Remove(transaction, "value2");
        map3.Insert(transaction, "value3", Input.Long(value: -1337L));

        transaction.Commit();

        // Assert
        AssertPath(called, pathSegments, "map-2", "map-3");
    }

    [Test]
    public void ObserveDeepHasMultipleEventsForMultipleInstanceChanges()
    {
        // Arrange
        var (doc, map1, map2, map3) = ArrangeDoc();

        IEnumerable<EventPath>? mapEvents = null;
        var called = 0;

        var subscription = map1.ObserveDeep(
            events =>
            {
                called++;
                mapEvents = events.Select(x => x.MapEvent.Path).ToArray();
            });

        // Act
        var transaction = doc.WriteTransaction();

        map1.Insert(transaction, "value", Input.Long(value: 2469L));
        map2.Insert(transaction, "value", Input.Long(value: -420L));
        map3.Insert(transaction, "value", Input.Boolean(value: true));

        transaction.Commit();

        // Assert
        AssertPath(called, mapEvents.ElementAt(index: 0));
        AssertPath(called, mapEvents.ElementAt(index: 1), "map-2");
        AssertPath(called, mapEvents.ElementAt(index: 2), "map-2", "map-3");
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

    private static void AssertPath(int called, IEnumerable<EventPathSegment>? pathSegments, params string[] path)
    {
        Assert.That(called, Is.EqualTo(expected: 1));
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
