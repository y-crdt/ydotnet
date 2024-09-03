using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;
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

        // Act
        undoEvent = null;
        transaction = doc.WriteTransaction();
        text.Insert(transaction, index: 0, " Viana");
        transaction.Commit();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Null);

        // Act
        undoEvent = null;
        transaction = doc.WriteTransaction();
        text.RemoveRange(transaction, index: 3, length: 5);
        transaction.Commit();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Null);
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
            transaction, index: 0, Input.Long(value: 2469L), Input.Boolean(value: false), Input.String("Lucas"));
        transaction.Commit();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Null);

        // Act
        undoEvent = null;
        transaction = doc.WriteTransaction();
        array.InsertRange(
            transaction, index: 2, Input.Bytes(new byte[] { 2, 4, 6, 9 }), Input.XmlText("Lucas"));
        transaction.Commit();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Null);

        // Act
        undoEvent = null;
        transaction = doc.WriteTransaction();
        array.RemoveRange(transaction, index: 1, length: 2);
        transaction.Commit();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Null);
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
    }

    [Test]
    public void TriggersAfterAddingAndRemovingContentOnXmlText()
    {
        // Arrange
        var doc = new Doc(new DocOptions { Id = 7853 });
        var xmlFragment = doc.XmlFragment("xml-fragment");

        var transaction = doc.WriteTransaction();
        var xmlText = xmlFragment.InsertText(transaction, index: 0);
        transaction.Commit();

        var undoManager = new UndoManager(
            doc, xmlText, new UndoManagerOptions
            {
                CaptureTimeoutMilliseconds = 0
            });

        UndoEvent? undoEvent = null;
        undoManager.ObserveAdded(e => undoEvent = e);

        // Act
        transaction = doc.WriteTransaction();
        xmlText.Insert(transaction, index: 0, "Lucas");
        transaction.Commit();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Null);

        // Act
        undoEvent = null;
        transaction = doc.WriteTransaction();
        xmlText.InsertAttribute(transaction, "bold", "true");
        transaction.Commit();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Null);

        // Act
        undoEvent = null;
        transaction = doc.WriteTransaction();
        xmlText.InsertEmbed(transaction, index: 2, Input.Bytes(new byte[] { 2, 4, 6, 9 }));
        transaction.Commit();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Null);

        // Act
        undoEvent = null;
        transaction = doc.WriteTransaction();
        xmlText.RemoveRange(transaction, index: 2, length: 2);
        transaction.Commit();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Null);
    }

    [Test]
    public void TriggersAfterAddingAndRemovingContentOnXmlElement()
    {
        // Arrange
        var doc = new Doc(new DocOptions { Id = 8137 });
        var xmlFragment = doc.XmlFragment("xml-fragment");

        var transaction = doc.WriteTransaction();
        var xmlElement = xmlFragment.InsertElement(transaction, index: 0, "xml-element");
        transaction.Commit();

        var undoManager = new UndoManager(
            doc, xmlElement, new UndoManagerOptions
            {
                CaptureTimeoutMilliseconds = 0
            });

        UndoEvent? undoEvent = null;
        undoManager.ObserveAdded(e => undoEvent = e);

        // Act
        transaction = doc.WriteTransaction();
        var xmlText = xmlElement.InsertText(transaction, index: 0);
        transaction.Commit();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Null);

        // Act
        undoEvent = null;
        transaction = doc.WriteTransaction();
        xmlText.Insert(transaction, index: 0, "Lucas");
        transaction.Commit();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Null);

        // Act
        undoEvent = null;
        transaction = doc.WriteTransaction();
        xmlElement.InsertAttribute(transaction, "italics", "true");
        transaction.Commit();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Null);

        // Act
        undoEvent = null;
        transaction = doc.WriteTransaction();
        var childXmlElement = xmlElement.InsertElement(transaction, index: 1, "color");
        transaction.Commit();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Null);

        // Act
        undoEvent = null;
        transaction = doc.WriteTransaction();
        var childXmlText = childXmlElement.InsertText(transaction, index: 0);
        childXmlText.Insert(transaction, index: 0, "Viana");
        transaction.Commit();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Null);

        // Act
        undoEvent = null;
        transaction = doc.WriteTransaction();
        xmlElement.RemoveRange(transaction, index: 1, length: 1);
        transaction.Commit();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
        Assert.That(undoEvent.Kind, Is.EqualTo(UndoEventKind.Redo));
        Assert.That(undoEvent.Origin, Is.Null);
    }
}
