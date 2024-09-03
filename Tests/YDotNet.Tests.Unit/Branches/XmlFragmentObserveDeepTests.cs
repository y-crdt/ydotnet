using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Types.Events;
using YDotNet.Document.Types.XmlElements;
using YDotNet.Document.Types.XmlFragments;

namespace YDotNet.Tests.Unit.Branches;

public class XmlFragmentObserveDeepTests
{
    [Test]
    public void ObserveDeepHasPathWhenAddedXmlElementsAndTexts()
    {
        // Arrange
        var (doc, xmlFragment, xmlElement1, _, xmlElement3) = ArrangeDoc();

        IEnumerable<EventPathSegment>? pathSegments = null;

        var subscription =
            xmlFragment.ObserveDeep(events => pathSegments = events.First().XmlElementEvent.Path.ToArray());

        // Act
        var transaction = doc.WriteTransaction();
        xmlElement3.InsertText(transaction, index: 0);
        transaction.Commit();

        // Assert
        Assert.That(pathSegments, Is.Not.Null);
        Assert.That(pathSegments.Count(), Is.EqualTo(expected: 3));
        Assert.That(pathSegments.All(x => x.Tag == EventPathSegmentTag.Index), Is.True);
        Assert.That(pathSegments.All(x => x.Key == null), Is.True);
        Assert.That(pathSegments.ElementAt(index: 0).Index, Is.EqualTo(expected: 0));
        Assert.That(pathSegments.ElementAt(index: 1).Index, Is.EqualTo(expected: 2));
        Assert.That(pathSegments.ElementAt(index: 2).Index, Is.EqualTo(expected: 1));

        // Act
        transaction = doc.WriteTransaction();
        xmlElement3.InsertElement(transaction, index: 1, "xml-element-4");
        transaction.Commit();

        // Assert
        Assert.That(pathSegments, Is.Not.Null);
        Assert.That(pathSegments.Count(), Is.EqualTo(expected: 3));
        Assert.That(pathSegments.All(x => x.Tag == EventPathSegmentTag.Index), Is.True);
        Assert.That(pathSegments.All(x => x.Key == null), Is.True);
        Assert.That(pathSegments.ElementAt(index: 0).Index, Is.EqualTo(expected: 0));
        Assert.That(pathSegments.ElementAt(index: 1).Index, Is.EqualTo(expected: 2));
        Assert.That(pathSegments.ElementAt(index: 2).Index, Is.EqualTo(expected: 1));
    }

    [Test]
    public void ObserveDeepHasPathWhenAddedAttributes()
    {
        // Arrange
        var (doc, xmlFragment, xmlElement1, _, xmlElement3) = ArrangeDoc();

        IEnumerable<EventPathSegment>? pathSegments = null;

        var subscription =
            xmlFragment.ObserveDeep(events => pathSegments = events.First().XmlElementEvent.Path.ToArray());

        // Act
        var transaction = doc.WriteTransaction();
        xmlElement3.InsertAttribute(transaction, "href", "https://lsviana.github.io/");
        transaction.Commit();

        // Assert
        Assert.That(pathSegments, Is.Not.Null);
        Assert.That(pathSegments.Count(), Is.EqualTo(expected: 3));
        Assert.That(pathSegments.All(x => x.Tag == EventPathSegmentTag.Index), Is.True);
        Assert.That(pathSegments.All(x => x.Key == null), Is.True);
        Assert.That(pathSegments.ElementAt(index: 0).Index, Is.EqualTo(expected: 0));
        Assert.That(pathSegments.ElementAt(index: 1).Index, Is.EqualTo(expected: 2));
        Assert.That(pathSegments.ElementAt(index: 2).Index, Is.EqualTo(expected: 1));
    }

    [Test]
    public void ObserveDeepHasPathWhenUpdatedAttributes()
    {
        // Arrange
        var (doc, xmlFragment, xmlElement1, _, xmlElement3) = ArrangeDoc();

        var transaction = doc.WriteTransaction();
        xmlElement3.InsertAttribute(transaction, "href", "https://lsviana.github.io/");
        transaction.Commit();

        IEnumerable<EventPathSegment>? pathSegments = null;

        var subscription =
            xmlFragment.ObserveDeep(events => pathSegments = events.First().XmlElementEvent.Path.ToArray());

        // Act
        transaction = doc.WriteTransaction();
        xmlElement3.InsertAttribute(transaction, "href", "https://github.com/LSViana/y-crdt");
        transaction.Commit();

        // Assert
        Assert.That(pathSegments, Is.Not.Null);
        Assert.That(pathSegments.Count(), Is.EqualTo(expected: 3));
        Assert.That(pathSegments.All(x => x.Tag == EventPathSegmentTag.Index), Is.True);
        Assert.That(pathSegments.All(x => x.Key == null), Is.True);
        Assert.That(pathSegments.ElementAt(index: 0).Index, Is.EqualTo(expected: 0));
        Assert.That(pathSegments.ElementAt(index: 1).Index, Is.EqualTo(expected: 2));
        Assert.That(pathSegments.ElementAt(index: 2).Index, Is.EqualTo(expected: 1));
    }

    [Test]
    public void ObserveDeepHasPathWhenRemovedAttributes()
    {
        // Arrange
        var (doc, xmlFragment, xmlElement1, _, xmlElement3) = ArrangeDoc();

        var transaction = doc.WriteTransaction();
        xmlElement3.InsertAttribute(transaction, "href", "https://lsviana.github.io/");
        transaction.Commit();

        IEnumerable<EventPathSegment>? pathSegments = null;

        var subscription =
            xmlFragment.ObserveDeep(events => pathSegments = events.First().XmlElementEvent.Path.ToArray());

        // Act
        transaction = doc.WriteTransaction();
        xmlElement3.RemoveAttribute(transaction, "href");
        transaction.Commit();

        // Assert
        Assert.That(pathSegments, Is.Not.Null);
        Assert.That(pathSegments.Count(), Is.EqualTo(expected: 3));
        Assert.That(pathSegments.All(x => x.Tag == EventPathSegmentTag.Index), Is.True);
        Assert.That(pathSegments.All(x => x.Key == null), Is.True);
        Assert.That(pathSegments.ElementAt(index: 0).Index, Is.EqualTo(expected: 0));
        Assert.That(pathSegments.ElementAt(index: 1).Index, Is.EqualTo(expected: 2));
        Assert.That(pathSegments.ElementAt(index: 2).Index, Is.EqualTo(expected: 1));
    }

    private (Doc, XmlFragment, XmlElement, XmlElement, XmlElement) ArrangeDoc()
    {
        var doc = new Doc();
        var xmlFragment = doc.XmlFragment("xml-fragment");

        var transaction = doc.WriteTransaction();
        var xmlElement1 = xmlFragment.InsertElement(transaction, index: 0, "xml-element-1");
        transaction.Commit();

        transaction = doc.WriteTransaction();
        xmlElement1.InsertText(transaction, index: 0);
        xmlElement1.InsertText(transaction, index: 1);
        var xmlElement2 = xmlElement1.InsertElement(transaction, index: 2, "xml-element-2");

        xmlElement2.InsertText(transaction, index: 0);
        var xmlElement3 = xmlElement2.InsertElement(transaction, index: 1, "xml-element-3");
        transaction.Commit();

        return (doc, xmlFragment, xmlElement1, xmlElement2, xmlElement3);
    }
}
