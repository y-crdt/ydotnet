using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Options;
using YDotNet.Document.UndoManagers;
using YDotNet.Document.UndoManagers.Events;

namespace YDotNet.Tests.Unit.UndoManagers;

public class ObserveAddedTests
{
    [Test]
    public void TriggersAfterAddingAndRemovingContentOnText()
    {
        // Arrange
        var doc = new Doc(
            new DocOptions
            {
                Id = 1234
            });
        var text = doc.Text("text");
        var undoManager = new UndoManager(
            doc, text, new UndoManagerOptions
            {
                CaptureTimeoutMilliseconds = 0
            });

        UndoEvent? undoEvent = null;

        undoManager.ObserveAdded(e => undoEvent = e);

        // Act
        var transaction = doc.WriteTransaction();
        text.Insert(transaction, index: 0, "Lucas");
        transaction.Commit();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Null);
        Assert.That(undoEvent.Deletions.Ranges, Is.Empty);
        Assert.That(undoEvent.Insertions.Ranges.Count, Is.EqualTo(expected: 1));
        Assert.That(undoEvent.Insertions.Ranges[key: 1234].Length, Is.EqualTo(expected: 1));
        Assert.That(undoEvent.Insertions.Ranges[key: 1234].ElementAt(index: 0).Start, Is.EqualTo(expected: 0));
        Assert.That(undoEvent.Insertions.Ranges[key: 1234].ElementAt(index: 0).End, Is.EqualTo(expected: 5));

        // Act
        transaction = doc.WriteTransaction();
        text.Insert(transaction, index: 0, " Viana");
        transaction.Commit();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Null);
        Assert.That(undoEvent.Deletions.Ranges, Is.Empty);
        Assert.That(undoEvent.Insertions.Ranges.Count, Is.EqualTo(expected: 1));
        Assert.That(undoEvent.Insertions.Ranges[key: 1234].Length, Is.EqualTo(expected: 1));
        Assert.That(undoEvent.Insertions.Ranges[key: 1234].ElementAt(index: 0).Start, Is.EqualTo(expected: 5));
        Assert.That(undoEvent.Insertions.Ranges[key: 1234].ElementAt(index: 0).End, Is.EqualTo(expected: 11));

        // Act
        transaction = doc.WriteTransaction();
        text.RemoveRange(transaction, index: 3, length: 5);
        transaction.Commit();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Null);
        Assert.That(undoEvent.Insertions.Ranges, Is.Empty);
        Assert.That(undoEvent.Deletions.Ranges.Count, Is.EqualTo(expected: 1));
        Assert.That(undoEvent.Deletions.Ranges[key: 1234].Length, Is.EqualTo(expected: 2));
        Assert.That(undoEvent.Deletions.Ranges[key: 1234].ElementAt(index: 0).Start, Is.EqualTo(expected: 0));
        Assert.That(undoEvent.Deletions.Ranges[key: 1234].ElementAt(index: 0).End, Is.EqualTo(expected: 2));
        Assert.That(undoEvent.Deletions.Ranges[key: 1234].ElementAt(index: 1).Start, Is.EqualTo(expected: 8));
        Assert.That(undoEvent.Deletions.Ranges[key: 1234].ElementAt(index: 1).End, Is.EqualTo(expected: 11));
    }

    [Test]
    [Ignore("To be implemented.")]
    public void TriggersAfterAddingAndRemovingContentOnArray()
    {
    }

    [Test]
    [Ignore("To be implemented.")]
    public void TriggersAfterAddingAndRemovingContentOnMap()
    {
    }

    [Test]
    [Ignore("To be implemented.")]
    public void TriggersAfterAddingAndRemovingContentOnXmlText()
    {
    }

    [Test]
    [Ignore("To be implemented.")]
    public void TriggersAfterAddingAndRemovingContentOnXmlElement()
    {
    }
}
