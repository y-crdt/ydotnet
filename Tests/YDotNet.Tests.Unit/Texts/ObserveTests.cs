using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;
using YDotNet.Document.Types.Events;
using YDotNet.Document.Types.Texts;

namespace YDotNet.Tests.Unit.Texts;

public class ObserveTests
{
    [Test]
    public void ObserveHasTarget()
    {
        // Arrange
        var doc = new Doc();
        var text = doc.Text("value");
        Text? target = null;
        text.Observe(e => target = e.Target);

        // Act
        var transaction = doc.WriteTransaction();
        text.Insert(transaction, index: 0, "Lucas");
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
        var text = doc.Text("value");
        EventDeltas? eventDeltas = null;
        text.Observe(e => eventDeltas = e.Delta);

        // Act
        var transaction = doc.WriteTransaction();
        text.Insert(transaction, index: 0, "Lucas");
        transaction.Commit();

        // Assert
        Assert.That(eventDeltas, Is.Not.Null);
        Assert.That(eventDeltas.Length, Is.EqualTo(expected: 1));
        Assert.That(eventDeltas.First().Tag, Is.EqualTo(EventDeltaTag.Add));
        Assert.That(eventDeltas.First().Length, Is.EqualTo(expected: 1));
        Assert.That(eventDeltas.First().Insert.String, Is.EqualTo("Lucas"));
        Assert.That(eventDeltas.First().Attributes, Is.Empty);
    }

    [Test]
    public void ObserveHasDeltasWhenRemoved()
    {
        // Arrange
        var doc = new Doc();
        var text = doc.Text("value");
        var transaction = doc.WriteTransaction();
        text.Insert(transaction, index: 0, "Lucas");
        transaction.Commit();

        EventDeltas? eventDeltas = null;
        text.Observe(e => eventDeltas = e.Delta);

        // Act
        transaction = doc.WriteTransaction();
        text.RemoveRange(transaction, index: 0, length: 2);
        transaction.Commit();

        // Assert
        Assert.That(eventDeltas, Is.Not.Null);
        Assert.That(eventDeltas.Length, Is.EqualTo(expected: 1));
        Assert.That(eventDeltas.First().Tag, Is.EqualTo(EventDeltaTag.Remove));
        Assert.That(eventDeltas.First().Length, Is.EqualTo(expected: 2));
        Assert.That(eventDeltas.First().Insert, Is.Null);
        Assert.That(eventDeltas.First().Attributes, Is.Empty);
    }

    [Test]
    public void ObserveHasDeltasWhenFormatted()
    {
        // Arrange
        var doc = new Doc();
        var text = doc.Text("value");
        var transaction = doc.WriteTransaction();
        text.Insert(transaction, index: 0, "Lucas");
        transaction.Commit();

        EventDeltas? eventDeltas = null;
        text.Observe(e => eventDeltas = e.Delta);

        // Act
        transaction = doc.WriteTransaction();
        text.Format(
            transaction, index: 0, length: 2, Input.Object(
                new Dictionary<string, Input>
                {
                    { "bold", Input.Boolean(value: true) }
                }));
        transaction.Commit();

        // Assert
        Assert.That(eventDeltas, Is.Not.Null);
        Assert.That(eventDeltas.Length, Is.EqualTo(expected: 1));
        Assert.That(eventDeltas.First().Tag, Is.EqualTo(EventDeltaTag.Retain));
        Assert.That(eventDeltas.First().Length, Is.EqualTo(expected: 2));
        Assert.That(eventDeltas.First().Insert, Is.Null);
        Assert.That(eventDeltas.First().Attributes.Count(), Is.EqualTo(expected: 1));
    }

    [Test]
    public void ObserveHasDeltasWhenAddedWithAttributes()
    {
        // Arrange
        var doc = new Doc();
        var text = doc.Text("value");
        EventDeltas? eventDeltas = null;
        text.Observe(e => eventDeltas = e.Delta);

        // Act
        var transaction = doc.WriteTransaction();
        text.Insert(
            transaction, index: 0, "Lucas", Input.Object(
                new Dictionary<string, Input>
                {
                    { "bold", Input.Boolean(value: true) }
                }));
        transaction.Commit();

        // Assert
        Assert.That(eventDeltas, Is.Not.Null);
        Assert.That(eventDeltas.Length, Is.EqualTo(expected: 1));
        Assert.That(eventDeltas.First().Tag, Is.EqualTo(EventDeltaTag.Add));
        Assert.That(eventDeltas.First().Length, Is.EqualTo(expected: 1));
        Assert.That(eventDeltas.First().Insert.String, Is.EqualTo("Lucas"));
        Assert.That(eventDeltas.First().Attributes.Count(), Is.EqualTo(expected: 1));
        Assert.That(eventDeltas.First().Attributes.First().Key, Is.EqualTo("bold"));
        Assert.That(eventDeltas.First().Attributes.First().Value.Boolean, Is.True);
        Assert.That(eventDeltas.First().Attributes.First().Value.Long, Is.Null);
    }
}
