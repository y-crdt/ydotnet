using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;
using YDotNet.Document.Events;
using YDotNet.Document.Options;
using YDotNet.Document.Types.Maps;

namespace YDotNet.Tests.Unit.Document;

public class SubDocsTests
{
    [Test]
    public void TriggersWhenAddingSubDocsUntilUnobserved()
    {
        // Arrange
        var doc = new Doc();
        var map = doc.Map("sub-docs");

        var called = 0;
        SubDocsEvent? subDocsEvent = null;

        var subscription = doc.ObserveSubDocs(
            e =>
            {
                called++;
                subDocsEvent = e;
            });

        // Act
        var subDoc1 = AddSubDoc(doc, map, "sub-doc-1");

        // Assert
        Assert.That(called, Is.EqualTo(expected: 1));
        Assert.That(subDocsEvent, Is.Not.Null);
        Assert.That(subDocsEvent.Added, Has.Length.EqualTo(expected: 1));
        Assert.That(subDocsEvent.Loaded, Has.Length.EqualTo(expected: 1));
        Assert.That(subDocsEvent.Removed, Is.Empty);
        Assert.That(subDocsEvent.Added[0].Id, Is.EqualTo(subDoc1.Id));

        // Act
        subDocsEvent = null;
        subscription.Dispose();

        AddSubDoc(doc, map, "sub-doc-2");

        // Assert
        Assert.That(called, Is.EqualTo(expected: 1));
        Assert.That(subDocsEvent, Is.Null);
    }

    [Test]
    public void TriggersWhenRemovingSubDocsUntilUnobserved()
    {
        // Arrange
        var doc = new Doc();
        var map = doc.Map("sub-docs");

        var subDoc1 = AddSubDoc(doc, map, "sub-doc-1");
        AddSubDoc(doc, map, "sub-doc-2");

        var called = 0;
        SubDocsEvent? subDocsEvent = null;

        var subscription = doc.ObserveSubDocs(
            e =>
            {
                called++;
                subDocsEvent = e;
            });

        // Act
        var removed = RemoveSubDoc(doc, map, "sub-doc-1");

        // Assert
        Assert.That(removed, Is.True);
        Assert.That(called, Is.EqualTo(expected: 1));
        Assert.That(subDocsEvent, Is.Not.Null);
        Assert.That(subDocsEvent.Added, Is.Empty);
        Assert.That(subDocsEvent.Loaded, Is.Empty);
        Assert.That(subDocsEvent.Removed, Has.Length.EqualTo(expected: 1));
        Assert.That(subDocsEvent.Removed[0].Id, Is.EqualTo(subDoc1.Id));

        // Act
        subDocsEvent = null;
        subscription.Dispose();

        RemoveSubDoc(doc, map, "sub-doc-2");

        // Assert
        Assert.That(called, Is.EqualTo(expected: 1));
        Assert.That(subDocsEvent, Is.Null);
    }

    [Test]
    public void TriggersWhenAddingSubDocsWithoutLoading()
    {
        // Arrange
        var doc = new Doc();
        var map = doc.Map("sub-docs");

        var called = 0;
        SubDocsEvent? subDocsEvent = null;

        var subscription = doc.ObserveSubDocs(
            e =>
            {
                called++;
                subDocsEvent = e;
            });

        // Act
        var subDoc1 = AddSubDocWithoutLoading(doc, map, "sub-doc-1");

        // Assert
        Assert.That(called, Is.EqualTo(expected: 1));
        Assert.That(subDocsEvent, Is.Not.Null);
        Assert.That(subDocsEvent.Added, Has.Length.EqualTo(expected: 1));
        Assert.That(subDocsEvent.Loaded, Is.Empty);
        Assert.That(subDocsEvent.Removed, Is.Empty);
        Assert.That(subDocsEvent.Added[0].Id, Is.EqualTo(subDoc1.Id));

        // Loading a document after they've been already added previously does
        // not trigger the event again, so this test does not cover that scenario.
    }

    private static Doc AddSubDocWithoutLoading(Doc doc, Map map, string key)
    {
        var transaction = doc.WriteTransaction();

        var subDoc1 = new Doc(
            new DocOptions
            {
                ShouldLoad = false
            });
        map.Insert(transaction, key, Input.Doc(subDoc1));

        transaction.Commit();

        return subDoc1;
    }

    private static Doc AddSubDoc(Doc doc, Map map, string key)
    {
        var transaction = doc.WriteTransaction();

        var subDoc1 = new Doc();
        map.Insert(transaction, key, Input.Doc(subDoc1));

        transaction.Commit();

        return subDoc1;
    }

    private static bool RemoveSubDoc(Doc doc, Map map, string key)
    {
        var transaction = doc.WriteTransaction();

        var result = map.Remove(transaction, key);

        transaction.Commit();

        return result;
    }
}
