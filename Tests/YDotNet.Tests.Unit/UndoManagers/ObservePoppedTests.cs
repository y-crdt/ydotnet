using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;
using YDotNet.Document.Options;
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

        // Act
        undoEvent = null;
        undoManager.Redo();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Not.Null);

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

        // Act
        undoEvent = null;
        undoManager.Redo();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Not.Null);
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
            transaction, index: 0, Input.Long(value: 2469L), Input.Boolean(value: false), Input.Undefined());
        transaction.Commit();
        undoManager.Undo();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Undo));
        Assert.That(undoEvent.Origin, Is.Not.Null);

        // Act
        undoEvent = null;
        undoManager.Redo();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Not.Null);

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

        // Act
        undoEvent = null;
        undoManager.Redo();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Not.Null);
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

        // Act
        undoEvent = null;
        undoManager.Redo();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Not.Null);

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

        // Act
        undoEvent = null;
        undoManager.Redo();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Not.Null);
    }

    [Test]
    public void TriggersAfterAddingAndRemovingContentOnXmlText()
    {
        // Arrange
        var doc = new Doc(new DocOptions { Id = 7938 });
        var xmlFragment = doc.XmlFragment("xml-fragment");

        var transaction = doc.WriteTransaction();
        var xmlText = xmlFragment.InsertText(transaction, index: 0);
        transaction.Commit();

        var undoManager = new UndoManager(doc, xmlText, new UndoManagerOptions { CaptureTimeoutMilliseconds = 0 });

        UndoEvent? undoEvent = null;
        undoManager.ObservePopped(e => undoEvent = e);

        // Act
        transaction = doc.WriteTransaction();
        xmlText.Insert(transaction, index: 0, "Lucas");
        transaction.Commit();
        undoManager.Undo();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Undo));
        Assert.That(undoEvent.Origin, Is.Not.Null);

        // Act
        GC.Collect();
        undoEvent = null;
        undoManager.Redo();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Not.Null);

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

        // Act
        undoEvent = null;
        undoManager.Redo();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Not.Null);

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

        // Act
        undoEvent = null;
        undoManager.Redo();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Not.Null);
    }

    [Test]
    public void TriggersAfterAddingAndRemovingContentOnXmlElement()
    {
        // Arrange
        var doc = new Doc(new DocOptions { Id = 5903 });
        var xmlFragment = doc.XmlFragment("xml-fragment");

        var transaction = doc.WriteTransaction();
        var xmlElement = xmlFragment.InsertElement(transaction, index: 0, "xml-element");
        transaction.Commit();

        var undoManager = new UndoManager(doc, xmlElement, new UndoManagerOptions { CaptureTimeoutMilliseconds = 0 });

        UndoEvent? undoEvent = null;
        undoManager.ObservePopped(e => undoEvent = e);

        // Act (add element and undo)
        transaction = doc.WriteTransaction();
        xmlElement.InsertElement(transaction, index: 0, "color");
        transaction.Commit();
        undoManager.Undo();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Undo));
        Assert.That(undoEvent.Origin, Is.Not.Null);

        GC.Collect();
        // Act (redo add element)
        undoEvent = null;
        undoManager.Redo();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Not.Null);

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

        // Act (redo add attribute)
        undoEvent = null;
        undoManager.Redo();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Not.Null);

        // Act (add text and undo)
        transaction = doc.WriteTransaction();
        xmlElement.InsertText(transaction, index: 1);
        transaction.Commit();
        undoManager.Undo();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Undo));
        Assert.That(undoEvent.Origin, Is.Not.Null);

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

        // Act (redo remove range)
        undoEvent = null;
        undoManager.Redo();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Not.Null);

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

        // Act (redo remove attribute)
        undoEvent = null;
        undoManager.Redo();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Not.Null);
    }
}
