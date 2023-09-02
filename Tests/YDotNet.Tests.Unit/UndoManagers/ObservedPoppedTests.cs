using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;
using YDotNet.Document.Options;
using YDotNet.Document.State;
using YDotNet.Document.UndoManagers;
using YDotNet.Document.UndoManagers.Events;

namespace YDotNet.Tests.Unit.UndoManagers;

public class ObservedPoppedTests
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
        var undoManager = new UndoManager(doc, text, new UndoManagerOptions { CaptureTimeoutMilliseconds = 0 });

        UndoEvent? undoEvent = null;
        undoManager.ObservePopped(e => undoEvent = e);

        // Act
        var transaction = doc.WriteTransaction();
        text.Insert(transaction, index: 0, "Lucas");
        transaction.Commit();
        undoManager.Undo();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Undo));
        Assert.That(undoEvent.Origin, Is.Not.Null);
        AssertDeleteSet(undoEvent.Deletions);
        AssertDeleteSet(undoEvent.Insertions, (1234, new[] { new IdRange(start: 0, end: 5) }));

        // Act
        undoEvent = null;
        undoManager.Redo();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Not.Null);
        AssertDeleteSet(undoEvent.Deletions, (1234, new[] { new IdRange(start: 0, end: 5) }));
        AssertDeleteSet(undoEvent.Insertions);

        // Act
        undoEvent = null;
        transaction = doc.WriteTransaction();
        text.RemoveRange(transaction, index: 3, length: 2);
        transaction.Commit();
        undoManager.Undo();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Undo));
        Assert.That(undoEvent.Origin, Is.Not.Null);

        // TODO [LSViana] Check with the team why the indexes are [8, 10] here.
        AssertDeleteSet(undoEvent.Deletions, (1234, new[] { new IdRange(start: 8, end: 10) }));
        AssertDeleteSet(undoEvent.Insertions);

        // Act
        undoEvent = null;
        undoManager.Redo();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Not.Null);
        AssertDeleteSet(undoEvent.Deletions);

        // TODO [LSViana] Check with the team why the indexes are [10, 12] here.
        AssertDeleteSet(undoEvent.Insertions, (1234, new[] { new IdRange(start: 10, end: 12) }));
    }

    [Test]
    public void TriggersAfterAddingAndRemovingContentOnArray()
    {
        // Arrange
        var doc = new Doc(
            new DocOptions
            {
                Id = 5678
            });
        var array = doc.Array("array");
        var undoManager = new UndoManager(doc, array, new UndoManagerOptions { CaptureTimeoutMilliseconds = 0 });

        UndoEvent? undoEvent = null;
        undoManager.ObservePopped(e => undoEvent = e);

        // Act
        var transaction = doc.WriteTransaction();
        array.InsertRange(
            transaction, index: 0, new[]
            {
                Input.Long(value: 2469L),
                Input.Boolean(value: false),
                Input.Undefined()
            });
        transaction.Commit();
        undoManager.Undo();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Undo));
        Assert.That(undoEvent.Origin, Is.Not.Null);
        AssertDeleteSet(undoEvent.Deletions);
        AssertDeleteSet(undoEvent.Insertions, (5678, new[] { new IdRange(start: 0, end: 3) }));

        // Act
        undoEvent = null;
        undoManager.Redo();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Not.Null);
        AssertDeleteSet(undoEvent.Deletions, (5678, new[] { new IdRange(start: 0, end: 3) }));
        AssertDeleteSet(undoEvent.Insertions);

        // Act
        undoEvent = null;
        transaction = doc.WriteTransaction();
        array.RemoveRange(transaction, index: 1, length: 2);
        transaction.Commit();
        undoManager.Undo();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Undo));
        Assert.That(undoEvent.Origin, Is.Not.Null);

        // TODO [LSViana] Check with the team why the indexes are [1, 4] here.
        AssertDeleteSet(undoEvent.Deletions, (5678, new[] { new IdRange(start: 4, end: 6) }));
        AssertDeleteSet(undoEvent.Insertions);

        // Act
        undoEvent = null;
        undoManager.Redo();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Not.Null);
        AssertDeleteSet(undoEvent.Deletions);

        // TODO [LSViana] Check with the team why the indexes are [6, 8] here.
        AssertDeleteSet(undoEvent.Insertions, (5678, new[] { new IdRange(start: 6, end: 8) }));
    }

    [Test]
    [Ignore("Waiting to be implemented.")]
    public void TriggersAfterAddingAndRemovingContentOnMap()
    {
    }

    [Test]
    [Ignore("Waiting to be implemented.")]
    public void TriggersAfterAddingAndRemovingContentOnXmlText()
    {
    }

    [Test]
    [Ignore("Waiting to be implemented.")]
    public void TriggersAfterAddingAndRemovingContentOnXmlElement()
    {
    }

    private void AssertDeleteSet(DeleteSet deleteSet, params (uint ClientId, IdRange[] IdRanges)[] idRangesPerClientId)
    {
        Assert.That(deleteSet.Ranges.Count, Is.EqualTo(idRangesPerClientId.Length));

        foreach (var entry in idRangesPerClientId)
        {
            var idRanges = deleteSet.Ranges[entry.ClientId];

            Assert.That(idRanges.Length, Is.EqualTo(entry.IdRanges.Length), "The amount of IdRanges is different.");

            for (var i = 0; i < idRanges.Length; i++)
            {
                Assert.That(
                    idRanges[i].Start, Is.EqualTo(entry.IdRanges[i].Start),
                    $"The start of the IdRange[{i}] is different.");
                Assert.That(
                    idRanges[i].End, Is.EqualTo(entry.IdRanges[i].End), $"The end of the IdRange[{i}] is different.");
            }
        }
    }
}
