using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Options;
using YDotNet.Document.State;
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
        AssertDeleteSet(undoEvent.Deletions);
        AssertDeleteSet(undoEvent.Insertions, (1234, new[] { new IdRange(start: 0, end: 5) }));

        // Act
        undoEvent = null;
        transaction = doc.WriteTransaction();
        text.Insert(transaction, index: 0, " Viana");
        transaction.Commit();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Null);
        AssertDeleteSet(undoEvent.Deletions);
        AssertDeleteSet(undoEvent.Insertions, (1234, new[] { new IdRange(start: 5, end: 11) }));

        // Act
        undoEvent = null;
        transaction = doc.WriteTransaction();
        text.RemoveRange(transaction, index: 3, length: 5);
        transaction.Commit();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Null);
        AssertDeleteSet(
            undoEvent.Deletions,
            (1234, new[] { new IdRange(start: 0, end: 2), new IdRange(start: 8, end: 11) }));
        AssertDeleteSet(undoEvent.Insertions);
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

    private void AssertDeleteSet(DeleteSet deleteSet, params (uint ClientId, IdRange[] IdRanges)[] idRangesPerClientId)
    {
        Assert.That(deleteSet.Ranges.Count, Is.EqualTo(idRangesPerClientId.Length));

        foreach (var entry in idRangesPerClientId)
        {
            var idRanges = deleteSet.Ranges[entry.ClientId];

            Assert.That(idRanges.Length, Is.EqualTo(entry.IdRanges.Length));

            for (var i = 0; i < idRanges.Length; i++)
            {
                Assert.That(idRanges[i].Start, Is.EqualTo(entry.IdRanges[i].Start));
                Assert.That(idRanges[i].End, Is.EqualTo(entry.IdRanges[i].End));
            }
        }
    }
}
