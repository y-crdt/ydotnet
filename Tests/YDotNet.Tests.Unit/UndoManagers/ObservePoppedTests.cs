using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;
using YDotNet.Document.Options;
using YDotNet.Document.State;
using YDotNet.Document.UndoManagers;
using YDotNet.Document.UndoManagers.Events;

namespace YDotNet.Tests.Unit.UndoManagers;

public class ObservePoppedTests
{
    [Test]
    public void TriggersAfterAddingAndRemovingContentOnText()
    {
        // Arrange
        var doc = new Doc(new DocOptions { Id = 1234 });
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
        AssertDeleteSet(undoEvent.Insertions, (1234, new[] { new IdRange(start: 10, end: 12) }));
    }

    [Test]
    public void TriggersAfterAddingAndRemovingContentOnArray()
    {
        // Arrange
        var doc = new Doc(new DocOptions { Id = 5678 });
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
        AssertDeleteSet(undoEvent.Insertions, (5678, new[] { new IdRange(start: 6, end: 8) }));
    }

    [Test]
    public void TriggersAfterAddingAndRemovingContentOnMap()
    {
        // Arrange
        var doc = new Doc(new DocOptions { Id = 9581 });
        var map = doc.Map("map");
        var undoManager = new UndoManager(doc, map, new UndoManagerOptions { CaptureTimeoutMilliseconds = 0 });

        UndoEvent? undoEvent = null;
        undoManager.ObservePopped(e => undoEvent = e);

        // Act
        var transaction = doc.WriteTransaction();
        map.Insert(transaction, "key1", Input.Boolean(value: false));
        transaction.Commit();
        undoManager.Undo();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Undo));
        Assert.That(undoEvent.Origin, Is.Not.Null);
        AssertDeleteSet(undoEvent.Deletions);
        AssertDeleteSet(undoEvent.Insertions, (9581, new[] { new IdRange(start: 0, end: 1) }));

        // Act
        undoEvent = null;
        undoManager.Redo();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Not.Null);
        AssertDeleteSet(undoEvent.Deletions, (9581, new[] { new IdRange(start: 0, end: 1) }));
        AssertDeleteSet(undoEvent.Insertions);

        // Act
        undoEvent = null;
        transaction = doc.WriteTransaction();
        map.Remove(transaction, "key1");
        transaction.Commit();
        undoManager.Undo();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Undo));
        Assert.That(undoEvent.Origin, Is.Not.Null);
        AssertDeleteSet(undoEvent.Deletions, (9581, new[] { new IdRange(start: 1, end: 2) }));
        AssertDeleteSet(undoEvent.Insertions);

        // Act
        undoEvent = null;
        undoManager.Redo();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Not.Null);
        AssertDeleteSet(undoEvent.Deletions);
        AssertDeleteSet(undoEvent.Insertions, (9581, new[] { new IdRange(start: 2, end: 3) }));
    }

    [Test]
    public void TriggersAfterAddingAndRemovingContentOnXmlText()
    {
        // Arrange
        var doc = new Doc(new DocOptions { Id = 7938 });
        var xmlText = doc.XmlText("xml-text");
        var undoManager = new UndoManager(doc, xmlText, new UndoManagerOptions { CaptureTimeoutMilliseconds = 0 });

        UndoEvent? undoEvent = null;
        undoManager.ObservePopped(e => undoEvent = e);

        // Act
        var transaction = doc.WriteTransaction();
        xmlText.Insert(transaction, index: 0, "Lucas");
        transaction.Commit();
        undoManager.Undo();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Undo));
        Assert.That(undoEvent.Origin, Is.Not.Null);
        AssertDeleteSet(undoEvent.Deletions);
        AssertDeleteSet(undoEvent.Insertions, (7938, new[] { new IdRange(start: 0, end: 5) }));

        // Act
        undoEvent = null;
        undoManager.Redo();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Not.Null);
        AssertDeleteSet(undoEvent.Deletions, (7938, new[] { new IdRange(start: 0, end: 5) }));
        AssertDeleteSet(undoEvent.Insertions);

        // Act
        undoEvent = null;
        transaction = doc.WriteTransaction();
        xmlText.InsertAttribute(transaction, "bold", "true");
        transaction.Commit();
        undoManager.Undo();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Undo));
        Assert.That(undoEvent.Origin, Is.Not.Null);
        AssertDeleteSet(undoEvent.Deletions);
        AssertDeleteSet(undoEvent.Insertions, (7938, new[] { new IdRange(start: 10, end: 11) }));

        // Act
        undoEvent = null;
        undoManager.Redo();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Not.Null);
        AssertDeleteSet(undoEvent.Deletions, (7938, new[] { new IdRange(start: 10, end: 11) }));
        AssertDeleteSet(undoEvent.Insertions);

        // Act
        undoEvent = null;
        transaction = doc.WriteTransaction();
        xmlText.RemoveRange(transaction, index: 2, length: 3);
        transaction.Commit();
        undoManager.Undo();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Undo));
        Assert.That(undoEvent.Origin, Is.Not.Null);
        AssertDeleteSet(undoEvent.Deletions, (7938, new[] { new IdRange(start: 7, end: 10) }));
        AssertDeleteSet(undoEvent.Insertions);

        // Act
        undoEvent = null;
        undoManager.Redo();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Not.Null);
        AssertDeleteSet(undoEvent.Deletions);
        AssertDeleteSet(undoEvent.Insertions, (7938, new[] { new IdRange(start: 12, end: 15) }));
    }

    [Test]
    public void TriggersAfterAddingAndRemovingContentOnXmlElement()
    {
        // Arrange
        var doc = new Doc(new DocOptions { Id = 5903 });
        var xmlElement = doc.XmlElement("xml-element");
        var undoManager = new UndoManager(doc, xmlElement, new UndoManagerOptions { CaptureTimeoutMilliseconds = 0 });

        UndoEvent? undoEvent = null;
        undoManager.ObservePopped(e => undoEvent = e);

        // Act (add element and undo)
        var transaction = doc.WriteTransaction();
        xmlElement.InsertElement(transaction, index: 0, "color");
        transaction.Commit();
        undoManager.Undo();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Undo));
        Assert.That(undoEvent.Origin, Is.Not.Null);
        AssertDeleteSet(undoEvent.Deletions);
        AssertDeleteSet(undoEvent.Insertions, (5903, new[] { new IdRange(start: 0, end: 1) }));

        // Act (redo add element)
        undoEvent = null;
        undoManager.Redo();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Not.Null);
        AssertDeleteSet(undoEvent.Deletions, (5903, new[] { new IdRange(start: 0, end: 1) }));
        AssertDeleteSet(undoEvent.Insertions);

        // Act (add attribute and undo)
        undoEvent = null;
        transaction = doc.WriteTransaction();
        xmlElement.InsertAttribute(transaction, "bold", "true");
        transaction.Commit();
        undoManager.Undo();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Undo));
        Assert.That(undoEvent.Origin, Is.Not.Null);
        AssertDeleteSet(undoEvent.Deletions);
        AssertDeleteSet(undoEvent.Insertions, (5903, new[] { new IdRange(start: 2, end: 3) }));

        // Act (redo add attribute)
        undoEvent = null;
        undoManager.Redo();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Not.Null);
        AssertDeleteSet(undoEvent.Deletions, (5903, new[] { new IdRange(start: 2, end: 3) }));
        AssertDeleteSet(undoEvent.Insertions);

        // Act (add text and undo)
        transaction = doc.WriteTransaction();
        xmlElement.InsertText(transaction, index: 1);
        transaction.Commit();
        undoManager.Undo();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Undo));
        Assert.That(undoEvent.Origin, Is.Not.Null);
        AssertDeleteSet(undoEvent.Deletions);
        AssertDeleteSet(undoEvent.Insertions, (5903, new[] { new IdRange(start: 4, end: 5) }));

        // Act (redo add text)
        undoEvent = null;
        undoManager.Redo();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Not.Null);

        // Act (remove range and undo)
        undoEvent = null;
        transaction = doc.WriteTransaction();
        xmlElement.RemoveRange(transaction, index: 0, length: 2);
        transaction.Commit();
        undoManager.Undo();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Undo));
        Assert.That(undoEvent.Origin, Is.Not.Null);
        AssertDeleteSet(
            undoEvent.Deletions, (5903, new[] { new IdRange(start: 1, end: 2), new IdRange(start: 5, end: 6) }));
        AssertDeleteSet(undoEvent.Insertions);

        // Act (redo remove range)
        undoEvent = null;
        undoManager.Redo();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Not.Null);
        AssertDeleteSet(undoEvent.Deletions);
        AssertDeleteSet(undoEvent.Insertions, (5903, new[] { new IdRange(start: 6, end: 8) }));

        // Act (remove attribute and undo)
        undoEvent = null;
        transaction = doc.WriteTransaction();
        xmlElement.RemoveAttribute(transaction, "bold");
        transaction.Commit();
        undoManager.Undo();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Undo));
        Assert.That(undoEvent.Origin, Is.Not.Null);
        AssertDeleteSet(undoEvent.Deletions, (5903, new[] { new IdRange(start: 3, end: 4) }));
        AssertDeleteSet(undoEvent.Insertions);

        // Act (redo remove attribute)
        undoEvent = null;
        undoManager.Redo();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Not.Null);
        AssertDeleteSet(undoEvent.Deletions);
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
