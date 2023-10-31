using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;
using YDotNet.Document.Types.Events;
using YDotNet.Document.Types.XmlTexts;

namespace YDotNet.Tests.Unit.XmlTexts;

public class ObserveTests
{
    [Test]
    public void ObserveHasTarget()
    {
        // Arrange
        var doc = new Doc();
        var xmlText = doc.XmlText("xml-text");

        XmlText? target = null;
        var subscription = xmlText.Observe(e => target = e.Target);

        // Act
        var transaction = doc.WriteTransaction();
        xmlText.Insert(transaction, index: 0, "Lucas");
        transaction.Commit();

        // Assert
        Assert.That(target, Is.Not.Null);
        Assert.That(target.Handle, Is.Not.EqualTo(nint.Zero));
    }

    [Test]
    public void ObserveHasDeltaWhenAddedTextsAndEmbeds()
    {
        // Arrange
        var doc = new Doc();
        var xmlText = doc.XmlText("xml-text");

        IEnumerable<EventDelta>? deltas = null;
        xmlText.Observe(e => deltas = e.Delta.ToArray());

        // Act
        var transaction = doc.WriteTransaction();
        xmlText.Insert(transaction, index: 0, "Lucas");
        xmlText.InsertEmbed(
            transaction, index: 3, Input.Boolean(value: true), Input.Object(
                new Dictionary<string, Input>
                {
                    { "bold", Input.Boolean(value: true) }
                }));
        transaction.Commit();

        // Assert
        Assert.That(deltas, Is.Not.Null);
        Assert.That(deltas.Count(), Is.EqualTo(expected: 3));
        Assert.That(deltas.All(x => x.Tag == EventDeltaTag.Add));

        var firstDelta = deltas.ElementAt(index: 0);
        var secondDelta = deltas.ElementAt(index: 1);
        var thirdDelta = deltas.ElementAt(index: 2);

        Assert.That(firstDelta.Length, Is.EqualTo(expected: 1));
        Assert.That(firstDelta.Insert.String, Is.EqualTo("Luc"));
        Assert.That(firstDelta.Attributes, Is.Empty);

        Assert.That(secondDelta.Length, Is.EqualTo(expected: 1));
        Assert.That(secondDelta.Insert.Boolean, Is.EqualTo(expected: true));
        Assert.That(secondDelta.Attributes.Count(), Is.EqualTo(expected: 1));
        Assert.That(secondDelta.Attributes.ElementAt(index: 0).Key, Is.EqualTo("bold"));
        Assert.That(secondDelta.Attributes.ElementAt(index: 0).Value.Boolean, Is.True);

        Assert.That(thirdDelta.Length, Is.EqualTo(expected: 1));
        Assert.That(thirdDelta.Insert.String, Is.EqualTo("as"));
        Assert.That(thirdDelta.Attributes, Is.Empty);
    }

    [Test]
    public void ObserveDoesNotHaveDeltaWhenAddedAttributes()
    {
        // Arrange
        var doc = new Doc();
        var xmlText = doc.XmlText("xml-text");

        IEnumerable<EventDelta>? deltas = null;
        xmlText.Observe(e => deltas = e.Delta.ToArray());

        // Act
        var transaction = doc.WriteTransaction();
        xmlText.InsertAttribute(transaction, "color", "red");
        transaction.Commit();

        // Assert
        Assert.That(deltas, Is.Not.Null);
        Assert.That(deltas, Is.Empty);
    }

    [Test]
    public void ObserveHasKeysWhenAddedAttributes()
    {
        // Arrange
        var doc = new Doc();
        var xmlText = doc.XmlText("xml-text");

        IEnumerable<EventKeyChange>? deltas = null;
        xmlText.Observe(e => deltas = e.Keys.ToArray());

        // Act
        var transaction = doc.WriteTransaction();
        xmlText.InsertAttribute(transaction, "color", "red");
        xmlText.InsertAttribute(transaction, "width", "16");
        transaction.Commit();

        // Assert
        Assert.That(deltas, Is.Not.Null);
        Assert.That(deltas.Count(), Is.EqualTo(expected: 2));
        Assert.That(deltas.All(x => x.Tag == EventKeyChangeTag.Add), Is.True);
        Assert.That(deltas.All(x => x.OldValue == null), Is.True);
        Assert.That(deltas.First(x => x.Key == "color").NewValue.String, Is.EqualTo("red"));
        Assert.That(deltas.First(x => x.Key == "width").NewValue.String, Is.EqualTo("16"));
    }

    [Test]
    public void ObserveHasKeysWhenUpdatedAttributes()
    {
        // Arrange
        var doc = new Doc();
        var xmlText = doc.XmlText("xml-text");

        var transaction = doc.WriteTransaction();
        xmlText.InsertAttribute(transaction, "color", "red");
        transaction.Commit();

        IEnumerable<EventKeyChange>? keyChanges = null;
        xmlText.Observe(e => keyChanges = e.Keys.ToArray());

        // Act
        transaction = doc.WriteTransaction();
        xmlText.InsertAttribute(transaction, "color", "blue");
        transaction.Commit();

        // Assert
        Assert.That(keyChanges, Is.Not.Null);
        Assert.That(keyChanges.Count(), Is.EqualTo(expected: 1));
        Assert.That(keyChanges.First().Tag, Is.EqualTo(EventKeyChangeTag.Update));
        Assert.That(keyChanges.First().NewValue.String, Is.EqualTo("blue"));
        Assert.That(keyChanges.First().OldValue.String, Is.EqualTo("red"));
    }

    [Test]
    public void ObserveHasKeysWhenRemovedAttributes()
    {
        // Arrange
        var doc = new Doc();
        var xmlText = doc.XmlText("xml-text");

        var transaction = doc.WriteTransaction();
        xmlText.InsertAttribute(transaction, "color", "red");
        transaction.Commit();

        IEnumerable<EventKeyChange>? keyChanges = null;
        xmlText.Observe(e => keyChanges = e.Keys.ToArray());

        // Act
        transaction = doc.WriteTransaction();
        xmlText.RemoveAttribute(transaction, "color");
        transaction.Commit();

        // Assert
        Assert.That(keyChanges, Is.Not.Null);
        Assert.That(keyChanges.Count(), Is.EqualTo(expected: 1));
        Assert.That(keyChanges.First().Tag, Is.EqualTo(EventKeyChangeTag.Remove));
        Assert.That(keyChanges.First().OldValue.String, Is.EqualTo("red"));
        Assert.That(keyChanges.First().NewValue, Is.Null);
    }

    [Test]
    public void ObserveDoesNotHaveKeysWhenAddedTextsAndEmbeds()
    {
        // Arrange
        var doc = new Doc();
        var xmlText = doc.XmlText("xml-text");

        IEnumerable<EventKeyChange>? keyChanges = null;
        xmlText.Observe(e => keyChanges = e.Keys.ToArray());

        // Act
        var transaction = doc.WriteTransaction();
        xmlText.Insert(transaction, index: 0, "Lucas");
        xmlText.InsertEmbed(transaction, index: 3, Input.Boolean(value: true));
        transaction.Commit();

        // Assert
        Assert.That(keyChanges, Is.Not.Null);
        Assert.That(keyChanges, Is.Empty);
    }
}
