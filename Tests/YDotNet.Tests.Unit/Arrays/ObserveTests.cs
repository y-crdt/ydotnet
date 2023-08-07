using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;
using YDotNet.Document.Types.Events;
using Array = YDotNet.Document.Types.Arrays.Array;

namespace YDotNet.Tests.Unit.Arrays;

public class ObserveTests
{
    [Test]
    public void ObserveHasTarget()
    {
        // Arrange
        var doc = new Doc();
        var array = doc.Array("array");
        Array? target = null;
        array.Observe(e => target = e.Target);

        // Act
        var transaction = doc.WriteTransaction();
        array.InsertRange(transaction, index: 0, new[] { Input.Long(value: 2469L) });
        transaction.Commit();

        // Assert
        Assert.That(target, Is.Not.Null);
        Assert.That(target.Handle, Is.Not.EqualTo(nint.Zero));
    }

    [Test]
    public void ObserveHasDeltasWhenAdded()
    {
        // Arrange
        var doc = new Doc();
        var array = doc.Array("array");
        EventChanges? eventChanges = null;
        var subscription = array.Observe(e => eventChanges = e.Delta);

        // Act
        var transaction = doc.WriteTransaction();
        array.InsertRange(transaction, index: 0, new[] { Input.Long(value: 2469L) });
        transaction.Commit();

        // Assert
        Assert.That(subscription.Id, Is.EqualTo(expected: 0L));
        Assert.That(eventChanges, Is.Not.Null);
        Assert.That(eventChanges.Length, Is.EqualTo(expected: 1));
        Assert.That(eventChanges.First().Tag, Is.EqualTo(EventChangeTag.Add));
        Assert.That(eventChanges.First().Length, Is.EqualTo(expected: 1));
        Assert.That(eventChanges.First().Values.First().Long, Is.EqualTo(expected: 2469L));
        Assert.That(eventChanges.First().Values.First().Double, Is.Null);
    }

    [Test]
    public void ObserveHasDeltasWhenRemoved()
    {
        // Arrange
        var doc = new Doc();
        var array = doc.Array("array");

        var transaction = doc.WriteTransaction();
        array.InsertRange(
            transaction, index: 0, new[]
            {
                Input.Long(value: 2469L),
                Input.Boolean(value: false),
                Input.Undefined()
            });
        transaction.Commit();

        EventChanges? eventChanges = null;
        var subscription = array.Observe(e => eventChanges = e.Delta);

        // Act
        transaction = doc.WriteTransaction();
        array.RemoveRange(transaction, index: 0, length: 2);
        transaction.Commit();

        // Assert
        Assert.That(subscription.Id, Is.EqualTo(expected: 0L));
        Assert.That(eventChanges, Is.Not.Null);
        Assert.That(eventChanges.Length, Is.EqualTo(expected: 1));
        Assert.That(eventChanges.First().Tag, Is.EqualTo(EventChangeTag.Remove));
        Assert.That(eventChanges.First().Length, Is.EqualTo(expected: 2));
        Assert.That(eventChanges.First().Values, Is.Empty);
    }

    [Test]
    public void ObserveHasDeltasWhenMoved()
    {
        // Arrange
        var doc = new Doc();
        var array = doc.Array("array");

        var transaction = doc.WriteTransaction();
        array.InsertRange(
            transaction, index: 0, new[]
            {
                Input.Long(value: 2469L),
                Input.Boolean(value: false),
                Input.Undefined()
            });
        transaction.Commit();

        EventChanges? eventChanges = null;
        var subscription = array.Observe(e => eventChanges = e.Delta);

        // Act
        transaction = doc.WriteTransaction();
        array.Move(transaction, sourceIndex: 2, targetIndex: 0);
        transaction.Commit();

        // Assert
        Assert.That(subscription.Id, Is.EqualTo(expected: 0L));
        Assert.That(eventChanges, Is.Not.Null);
        Assert.That(eventChanges.Length, Is.EqualTo(expected: 3));

        Assert.That(eventChanges.ElementAt(index: 0).Tag, Is.EqualTo(EventChangeTag.Add));
        Assert.That(eventChanges.ElementAt(index: 0).Length, Is.EqualTo(expected: 1));
        Assert.That(eventChanges.ElementAt(index: 0).Values.First().Undefined, Is.True);
        Assert.That(eventChanges.ElementAt(index: 0).Values.First().Double, Is.Null);

        Assert.That(eventChanges.ElementAt(index: 1).Tag, Is.EqualTo(EventChangeTag.Retain));
        Assert.That(eventChanges.ElementAt(index: 1).Length, Is.EqualTo(expected: 2));
        Assert.That(eventChanges.ElementAt(index: 1).Values, Is.Empty);

        Assert.That(eventChanges.ElementAt(index: 2).Tag, Is.EqualTo(EventChangeTag.Remove));
        Assert.That(eventChanges.ElementAt(index: 2).Length, Is.EqualTo(expected: 1));
        Assert.That(eventChanges.ElementAt(index: 2).Values, Is.Empty);
    }
}
