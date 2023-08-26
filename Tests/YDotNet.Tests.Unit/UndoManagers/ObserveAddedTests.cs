using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;
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

        // TODO [LSViana] Check with the team why the indexes are [0, 2] and [8, 11] here.
        AssertDeleteSet(
            undoEvent.Deletions,
            (1234, new[] { new IdRange(start: 0, end: 2), new IdRange(start: 8, end: 11) }));
        AssertDeleteSet(undoEvent.Insertions);
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
        var undoManager = new UndoManager(
            doc, array, new UndoManagerOptions
            {
                CaptureTimeoutMilliseconds = 0
            });

        UndoEvent? undoEvent = null;
        undoManager.ObserveAdded(e => undoEvent = e);

        // Act
        var transaction = doc.WriteTransaction();
        array.InsertRange(
            transaction, index: 0, new[]
            {
                Input.Long(value: 2469L),
                Input.Boolean(value: false),
                Input.String("Lucas")
            });
        transaction.Commit();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Null);
        AssertDeleteSet(undoEvent.Deletions);
        AssertDeleteSet(undoEvent.Insertions, (5678, new[] { new IdRange(start: 0, end: 3) }));

        // Act
        undoEvent = null;
        transaction = doc.WriteTransaction();
        array.InsertRange(
            transaction, index: 2, new[]
            {
                Input.Bytes(new byte[] { 2, 4, 6, 9, 123, 123 }),
                Input.XmlText("Lucas")
            });
        transaction.Commit();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Null);
        AssertDeleteSet(undoEvent.Deletions);

        // TODO [LSViana] Check with the team why the indexes are [3, 10] here.
        AssertDeleteSet(undoEvent.Insertions, (5678, new[] { new IdRange(start: 3, end: 10) }));

        // Act
        undoEvent = null;
        transaction = doc.WriteTransaction();
        array.RemoveRange(transaction, index: 1, length: 2);
        transaction.Commit();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Null);
        AssertDeleteSet(undoEvent.Insertions);

        // TODO [LSViana] Check with the team why the indexes are [1, 2] and [3, 4] here.
        AssertDeleteSet(
            undoEvent.Deletions, (5678, new[] { new IdRange(start: 1, end: 2), new IdRange(start: 3, end: 4) }));
    }

    [Test]
    public void TriggersAfterAddingAndRemovingContentOnMap()
    {
        // Arrange
        var doc = new Doc(new DocOptions { Id = 9012 });
        var map = doc.Map("map");
        var undoManager = new UndoManager(
            doc, map, new UndoManagerOptions
            {
                CaptureTimeoutMilliseconds = 0
            });

        UndoEvent? undoEvent = null;
        undoManager.ObserveAdded(e => undoEvent = e);

        // Act
        var transaction = doc.WriteTransaction();
        map.Insert(transaction, "key1", Input.Long(value: 2469L));
        map.Insert(transaction, "key2", Input.Boolean(value: false));
        map.Insert(transaction, "key3", Input.String("Lucas"));
        transaction.Commit();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Null);
        AssertDeleteSet(undoEvent.Deletions);
        AssertDeleteSet(undoEvent.Insertions, (9012, new[] { new IdRange(start: 0, end: 3) }));

        // Act
        undoEvent = null;
        transaction = doc.WriteTransaction();
        map.Insert(transaction, "key4", Input.Bytes(new byte[] { 2, 4, 6, 9, 123, 123 }));
        map.Insert(transaction, "key5", Input.XmlText("Lucas"));
        transaction.Commit();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Null);
        AssertDeleteSet(undoEvent.Deletions);

        // TODO [LSViana] Check with the team why the indexes are [0, 3] here.
        AssertDeleteSet(undoEvent.Insertions, (9012, new[] { new IdRange(start: 3, end: 10) }));

        // Act
        undoEvent = null;
        transaction = doc.WriteTransaction();
        map.Remove(transaction, "key2");
        map.Remove(transaction, "key4");
        transaction.Commit();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Null);
        AssertDeleteSet(undoEvent.Insertions);
        AssertDeleteSet(
            undoEvent.Deletions, (9012, new[] { new IdRange(start: 1, end: 2), new IdRange(start: 3, end: 4) }));
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
