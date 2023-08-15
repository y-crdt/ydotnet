using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Types.Events;
using YDotNet.Document.Types.XmlElements;

namespace YDotNet.Tests.Unit.XmlElements;

public class ObserveTests
{
    [Test]
    public void ObserveHasTarget()
    {
        // Arrange
        var doc = new Doc();
        var parentXmlElement = doc.XmlElement("xml-element");

        // TODO [LSViana] Check with the team why the event can't be generated for root nodes.
        var transaction = doc.WriteTransaction();
        var xmlElement = parentXmlElement.InsertElement(transaction, index: 0, "color");
        transaction.Commit();

        XmlElement? target = null;
        xmlElement.Observe(e => target = e.Target);

        // Act
        transaction = doc.WriteTransaction();
        xmlElement.InsertText(transaction, index: 0);
        transaction.Commit();

        // Assert
        Assert.That(target, Is.Not.Null);
        Assert.That(target.Handle, Is.Not.EqualTo(nint.Zero));
    }

    [Test]
    public void ObserveHasDeltaWhenAddedXmlElementsAndTexts()
    {
        // Arrange
        var doc = new Doc();
        var parentXmlElement = doc.XmlElement("xml-element");

        // TODO [LSViana] Check with the team why the event can't be generated for root nodes.
        var transaction = doc.WriteTransaction();
        var xmlElement = parentXmlElement.InsertElement(transaction, index: 0, "color");
        transaction.Commit();

        IEnumerable<EventChange>? eventChanges = null;
        xmlElement.Observe(e => eventChanges = e.Delta.ToArray());

        // Act
        transaction = doc.WriteTransaction();
        xmlElement.InsertText(transaction, index: 0);
        xmlElement.InsertElement(transaction, index: 1, "color");
        xmlElement.InsertText(transaction, index: 2);
        transaction.Commit();

        // Assert
        Assert.That(eventChanges, Is.Not.Null);
        Assert.That(eventChanges.Count(), Is.EqualTo(expected: 1));
        Assert.That(eventChanges.First().Tag, Is.EqualTo(EventChangeTag.Add));
        Assert.That(eventChanges.First().Length, Is.EqualTo(expected: 3));
        Assert.That(eventChanges.First().Values.ElementAt(index: 0).XmlText, Is.Not.Null);
        Assert.That(eventChanges.First().Values.ElementAt(index: 0).XmlElement, Is.Null);
        Assert.That(eventChanges.First().Values.ElementAt(index: 1).XmlText, Is.Null);
        Assert.That(eventChanges.First().Values.ElementAt(index: 1).XmlElement, Is.Not.Null);
        Assert.That(eventChanges.First().Values.ElementAt(index: 1).XmlElement.Tag, Is.EqualTo("color"));
        Assert.That(eventChanges.First().Values.ElementAt(index: 2).XmlText, Is.Not.Null);
        Assert.That(eventChanges.First().Values.ElementAt(index: 2).XmlElement, Is.Null);
    }

    [Test]
    public void ObserveDoesNotHaveDeltaWhenAddedAttributes()
    {
        // Arrange
        var doc = new Doc();
        var parentXmlElement = doc.XmlElement("xml-element");

        // TODO [LSViana] Check with the team why the event can't be generated for root nodes.
        var transaction = doc.WriteTransaction();
        var xmlElement = parentXmlElement.InsertElement(transaction, index: 0, "color");
        transaction.Commit();

        IEnumerable<EventChange>? eventChanges = null;
        xmlElement.Observe(e => eventChanges = e.Delta.ToArray());

        // Act
        transaction = doc.WriteTransaction();
        xmlElement.InsertAttribute(transaction, "href", "https://lsviana.github.io/");
        transaction.Commit();

        // Assert
        Assert.That(eventChanges, Is.Not.Null);
        Assert.That(eventChanges.Count(), Is.EqualTo(expected: 0));
    }

    [Test]
    public void ObserveHasKeysWhenAddedAttributes()
    {
        // Arrange
        var doc = new Doc();
        var parentXmlElement = doc.XmlElement("xml-element");

        // TODO [LSViana] Check with the team why the event can't be generated for root nodes.
        var transaction = doc.WriteTransaction();
        var xmlElement = parentXmlElement.InsertElement(transaction, index: 0, "color");
        transaction.Commit();

        IEnumerable<EventKeyChange>? keyChanges = null;
        xmlElement.Observe(e => keyChanges = e.Keys.ToArray());

        // Act
        transaction = doc.WriteTransaction();
        xmlElement.InsertAttribute(transaction, "href", "https://lsviana.github.io/");
        xmlElement.InsertAttribute(transaction, "rel", "preload");
        xmlElement.InsertAttribute(transaction, "as", "document");
        transaction.Commit();

        // Assert
        Assert.That(keyChanges, Is.Not.Null);
        Assert.That(keyChanges.Count(), Is.EqualTo(expected: 3));
        Assert.That(keyChanges.All(x => x.Tag == EventKeyChangeTag.Add), Is.True);
        Assert.That(keyChanges.All(x => x.OldValue == null), Is.True);

        var hrefChange = keyChanges.Single(x => x.Key == "href");
        var relChange = keyChanges.Single(x => x.Key == "rel");
        var asChange = keyChanges.Single(x => x.Key == "as");

        Assert.That(hrefChange.NewValue.String, Is.EqualTo("https://lsviana.github.io/"));
        Assert.That(hrefChange.NewValue.Long, Is.Null);
        Assert.That(relChange.NewValue.String, Is.EqualTo("preload"));
        Assert.That(relChange.NewValue.Long, Is.Null);
        Assert.That(asChange.NewValue.String, Is.EqualTo("document"));
        Assert.That(asChange.NewValue.Long, Is.Null);
    }

    [Test]
    public void ObserveHasKeysWhenUpdatedAttributes()
    {
        // Arrange
        var doc = new Doc();
        var parentXmlElement = doc.XmlElement("xml-element");

        // TODO [LSViana] Check with the team why the event can't be generated for root nodes.
        var transaction = doc.WriteTransaction();
        var xmlElement = parentXmlElement.InsertElement(transaction, index: 0, "color");
        xmlElement.InsertAttribute(transaction, "href", "https://lsviana.github.io/");
        transaction.Commit();

        IEnumerable<EventKeyChange>? keyChanges = null;
        xmlElement.Observe(e => keyChanges = e.Keys.ToArray());

        // Act
        transaction = doc.WriteTransaction();
        xmlElement.InsertAttribute(transaction, "href", "https://github.com/LSViana/y-crdt");
        transaction.Commit();

        // Assert
        Assert.That(keyChanges, Is.Not.Null);
        Assert.That(keyChanges.Count(), Is.EqualTo(expected: 1));
        Assert.That(keyChanges.ElementAt(index: 0).Tag, Is.EqualTo(EventKeyChangeTag.Update));
        Assert.That(keyChanges.ElementAt(index: 0).Key, Is.EqualTo("href"));
        Assert.That(keyChanges.ElementAt(index: 0).NewValue.String, Is.EqualTo("https://github.com/LSViana/y-crdt"));
        Assert.That(keyChanges.ElementAt(index: 0).NewValue.Long, Is.Null);
        Assert.That(keyChanges.ElementAt(index: 0).OldValue.String, Is.EqualTo("https://lsviana.github.io/"));
        Assert.That(keyChanges.ElementAt(index: 0).OldValue.Long, Is.Null);
    }

    [Test]
    public void ObserveHasKeysWhenRemovedAttributes()
    {
        // Arrange
        var doc = new Doc();
        var parentXmlElement = doc.XmlElement("xml-element");

        // TODO [LSViana] Check with the team why the event can't be generated for root nodes.
        var transaction = doc.WriteTransaction();
        var xmlElement = parentXmlElement.InsertElement(transaction, index: 0, "color");
        xmlElement.InsertAttribute(transaction, "href", "https://lsviana.github.io/");
        transaction.Commit();

        IEnumerable<EventKeyChange>? keyChanges = null;
        xmlElement.Observe(e => keyChanges = e.Keys.ToArray());

        // Act
        transaction = doc.WriteTransaction();
        xmlElement.RemoveAttribute(transaction, "href");
        transaction.Commit();

        // Assert
        Assert.That(keyChanges, Is.Not.Null);
        Assert.That(keyChanges.Count(), Is.EqualTo(expected: 1));
        Assert.That(keyChanges.ElementAt(index: 0).Tag, Is.EqualTo(EventKeyChangeTag.Remove));
        Assert.That(keyChanges.ElementAt(index: 0).Key, Is.EqualTo("href"));
        Assert.That(keyChanges.ElementAt(index: 0).NewValue, Is.Null);
        Assert.That(keyChanges.ElementAt(index: 0).OldValue.String, Is.EqualTo("https://lsviana.github.io/"));
        Assert.That(keyChanges.ElementAt(index: 0).OldValue.Long, Is.Null);
    }

    [Test]
    public void ObserveDoesNotHaveKeysWhenAddedXmlElementsAndTexts()
    {
        // Arrange
        var doc = new Doc();
        var parentXmlElement = doc.XmlElement("xml-element");

        // TODO [LSViana] Check with the team why the event can't be generated for root nodes.
        var transaction = doc.WriteTransaction();
        var xmlElement = parentXmlElement.InsertElement(transaction, index: 0, "color");
        transaction.Commit();

        IEnumerable<EventKeyChange>? keyChanges = null;
        xmlElement.Observe(e => keyChanges = e.Keys.ToArray());

        // Act
        transaction = doc.WriteTransaction();
        xmlElement.InsertText(transaction, index: 0);
        xmlElement.InsertElement(transaction, index: 1, "color");
        xmlElement.InsertText(transaction, index: 2);
        transaction.Commit();

        // Assert
        Assert.That(keyChanges, Is.Not.Null);
        Assert.That(keyChanges.Count(), Is.EqualTo(expected: 0));
    }
}
