using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;
using YDotNet.Document.Types.Events;

namespace YDotNet.Tests.Unit.Arrays;

public class ObserveDeepTests
{
    [Test]
    public void ObserveDeepHasPathWhenAdded()
    {
        // Arrange
        var doc = new Doc();
        var array1 = doc.Array("array-1");

        var transaction = doc.WriteTransaction();
        array1.InsertRange(
            transaction, index: 0, new[]
            {
                Input.Long(value: 2469L),
                Input.Null(),
                Input.Boolean(value: false),
                Input.Map(new Dictionary<string, Input>())
            });

        var map2 = array1.Get(transaction, index: 3).Map;
        map2.Insert(transaction, "array-3", Input.Array(Array.Empty<Input>()));

        var array3 = map2.Get(transaction, "array-3").Array;
        array3.InsertRange(
            transaction, index: 0, new[]
            {
                Input.Boolean(value: false),
                Input.Long(value: 2469L),
                Input.Null()
            });
        transaction.Commit();

        IEnumerable<EventPathSegment>? pathSegments = null;
        var called = 0;

        var subscription = array1.ObserveDeep(
            events =>
            {
                called++;
                pathSegments = events.First().ArrayEvent.Path.ToArray();
            });

        // Act
        transaction = doc.WriteTransaction();
        array3.InsertRange(transaction, index: 3, new[] { Input.Double(value: 4.20) });
        transaction.Commit();

        // Assert
        Assert.That(pathSegments.Count(), Is.EqualTo(expected: 2));

        Assert.That(pathSegments.ElementAt(index: 0).Tag, Is.EqualTo(EventPathSegmentTag.Index));

        // TODO [LSViana] Check with the team why this isn't "3", since this is the index the item is being added.
        Assert.That(pathSegments.ElementAt(index: 0).Index, Is.EqualTo(expected: 1));
        Assert.That(pathSegments.ElementAt(index: 0).Key, Is.Null);

        Assert.That(pathSegments.ElementAt(index: 1).Tag, Is.EqualTo(EventPathSegmentTag.Key));
        Assert.That(pathSegments.ElementAt(index: 1).Key, Is.EqualTo("array-3"));
        Assert.That(pathSegments.ElementAt(index: 1).Index, Is.Null);

        array1.UnobserveDeep(subscription);
    }

    [Test]
    public void ObserveDeepHasPathWhenRemoved()
    {
        // Arrange
        var doc = new Doc();
        var array1 = doc.Array("array-1");

        var transaction = doc.WriteTransaction();
        array1.InsertRange(
            transaction, index: 0, new[]
            {
                Input.Long(value: 2469L),
                Input.Null(),
                Input.Array(Array.Empty<Input>()),
                Input.Boolean(value: false)
            });

        var array2 = array1.Get(transaction, index: 2).Array;
        array2.InsertRange(
            transaction, index: 0, new[]
            {
                Input.Long(value: 2469L),
                Input.Null(),
                Input.Boolean(value: false),
                Input.Array(Array.Empty<Input>())
            });

        var array3 = array2.Get(transaction, index: 3).Array;
        array3.InsertRange(
            transaction, index: 0, new[]
            {
                Input.Boolean(value: false),
                Input.Long(value: 2469L),
                Input.Null()
            });
        transaction.Commit();

        IEnumerable<EventPathSegment>? pathSegments = null;
        var called = 0;

        var subscription = array1.ObserveDeep(
            events =>
            {
                called++;
                pathSegments = events.First().ArrayEvent.Path.ToArray();
            });

        // Act
        transaction = doc.WriteTransaction();
        array3.RemoveRange(transaction, index: 2, length: 1);
        transaction.Commit();

        // Assert
        Assert.That(pathSegments.Count(), Is.EqualTo(expected: 2));

        Assert.That(pathSegments.ElementAt(index: 0).Tag, Is.EqualTo(EventPathSegmentTag.Index));
        // TODO [LSViana] Check with the team why this isn't "2", since this is the index the item is being removed.
        Assert.That(pathSegments.ElementAt(index: 0).Index, Is.EqualTo(expected: 1));
        Assert.That(pathSegments.ElementAt(index: 0).Key, Is.Null);

        Assert.That(pathSegments.ElementAt(index: 1).Tag, Is.EqualTo(EventPathSegmentTag.Index));
        // TODO [LSViana] Check with the team why this isn't "3", since this is the index inside "array2".
        Assert.That(pathSegments.ElementAt(index: 1).Index, Is.EqualTo(expected: 1));
        Assert.That(pathSegments.ElementAt(index: 1).Key, Is.Null);

        array1.UnobserveDeep(subscription);
    }
}
