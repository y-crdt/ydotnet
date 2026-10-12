using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;
using YDotNet.Document.Events;
using YDotNet.Document.Options;

namespace YDotNet.Tests.Unit.Document;

public class ClearTests
{
    [Test]
    public void Clear()
    {
        // Arrange and Act
        var doc = new Doc();

        // Assert
        doc.Clear();
    }

    [Test]
    public void TriggersWhenObserved()
    {
        // Arrange
        var doc = new Doc();

        ClearEvent? clearEvent = null;
        var called = 0;

        var subscription = doc.ObserveClear(
            e =>
            {
                called++;
                clearEvent = e;
            });

        // Act
        doc.Clear();

        // Assert
        Assert.That(called, Is.EqualTo(expected: 1));
        Assert.That(clearEvent, Is.Not.Null);
        Assert.That(clearEvent.Doc.Id, Is.EqualTo(doc.Id));
    }

    [Test]
    public void DoesNotTriggerWhenUnobserved()
    {
        // Arrange
        var doc = new Doc();

        ClearEvent? clearEvent = null;
        var called = 0;

        var subscription = doc.ObserveClear(
            e =>
            {
                called++;
                clearEvent = e;
            });

        // Act
        subscription.Dispose();
        doc.Clear();

        // Assert
        Assert.That(called, Is.EqualTo(expected: 0));
        Assert.That(clearEvent, Is.Null);
    }

    [Test]
    public void ClearsManyDocumentsWithContent()
    {
        // Arrange and Act
        for (var i = 0; i < 50; i++)
        {
            var doc = new Doc();
            var text = doc.Text("text");

            using (var transaction = doc.WriteTransaction())
            {
                text.Insert(transaction, index: 0, "content");
            }

            // Assert
            Assert.DoesNotThrow(doc.Clear);
        }
    }

    [Test]
    public void ClearsSubDocumentReadFromParent()
    {
        // Arrange
        var doc = new Doc();
        var map = doc.Map("sub-docs");

        using (var transaction = doc.WriteTransaction())
        {
            map.Insert(transaction, "sub", Input.Doc(new Doc()));
        }

        Doc subDoc;
        using (var transaction = doc.ReadTransaction())
        {
            subDoc = map.Get(transaction, "sub")!.Doc;
        }

        var called = 0;
        subDoc.ObserveClear(_ => called++);

        // Act
        subDoc.Clear();

        // Assert
        Assert.That(called, Is.EqualTo(expected: 1));
    }

    [Test]
    public void ReportsLoadFlagsFromOptions()
    {
        // Arrange
        var loaded = new Doc(new DocOptions { ShouldLoad = true, AutoLoad = true });
        var notLoaded = new Doc(new DocOptions { ShouldLoad = false, AutoLoad = false });

        // Assert
        Assert.That(loaded.ShouldLoad, Is.True);
        Assert.That(loaded.AutoLoad, Is.True);
        Assert.That(notLoaded.ShouldLoad, Is.False);
        Assert.That(notLoaded.AutoLoad, Is.False);
    }
}
