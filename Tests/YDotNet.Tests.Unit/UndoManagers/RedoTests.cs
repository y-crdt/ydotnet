using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;
using YDotNet.Document.UndoManagers;

namespace YDotNet.Tests.Unit.UndoManagers;

public class RedoTests
{
    [Test]
    public void ReturnsFalseWhenNoChangesApplied()
    {
        // Arrange
        var doc = new Doc();
        var text = doc.Text("text");
        var undoManager = new UndoManager(doc, text, new UndoManagerOptions { CaptureTimeoutMilliseconds = 0 });

        // Act
        var result = undoManager.Redo();

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void RedoAddingAndUpdatingAndRemovingContentOnText()
    {
        // Arrange
        var doc = new Doc();
        var text = doc.Text("text");
        var undoManager = new UndoManager(doc, text, new UndoManagerOptions { CaptureTimeoutMilliseconds = 0 });
        var transaction = doc.WriteTransaction();
        text.Insert(transaction, index: 0, "Lucas");
        transaction.Commit();

        // Act (add text, undo, and redo)
        transaction = doc.WriteTransaction();
        text.Insert(transaction, index: 5, " Viana");
        transaction.Commit();
        undoManager.Undo();
        var result = undoManager.Redo();

        transaction = doc.ReadTransaction();
        var value = text.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(value, Is.EqualTo("Lucas Viana"));
        Assert.That(result, Is.True);

        // Act (add embed, undo, and redo)
        transaction = doc.WriteTransaction();
        text.InsertEmbed(
            transaction, index: 3, Input.Boolean(value: true), Input.Object(
                new Dictionary<string, Input>
                {
                    { "bold", Input.Boolean(value: true) }
                }));
        transaction.Commit();
        undoManager.Redo();
        result = undoManager.Redo();

        transaction = doc.ReadTransaction();
        var chunks = text.Chunks(transaction).ToArray();
        transaction.Commit();

        // Assert
        Assert.That(chunks.Length, Is.EqualTo(expected: 3));
        Assert.That(result, Is.False);

        // Act (remove, undo, and redo)
        transaction = doc.WriteTransaction();
        text.RemoveRange(transaction, index: 2, length: 2);
        transaction.Commit();
        undoManager.Undo();
        result = undoManager.Redo();

        transaction = doc.ReadTransaction();
        value = text.String(transaction);
        chunks = text.Chunks(transaction).ToArray();
        transaction.Commit();

        // Assert
        Assert.That(value, Is.EqualTo("Luas Viana"));
        Assert.That(chunks.Length, Is.EqualTo(expected: 2));
        Assert.That(result, Is.True);
    }

    [Test]
    public void RedoAddingAndUpdatingAndRemovingContentOnArray()
    {
        // Arrange
        var doc = new Doc();
        var array = doc.Array("array");
        var undoManager = new UndoManager(doc, array, new UndoManagerOptions { CaptureTimeoutMilliseconds = 0 });
        var transaction = doc.WriteTransaction();
        array.InsertRange(
            transaction,
            index: 0,
            new[]
            {
                Input.Boolean(value: true),
                Input.Long(value: 2469L),
                Input.String("Lucas")
            });
        transaction.Commit();

        // Act (add, undo, and redo)
        transaction = doc.WriteTransaction();
        array.InsertRange(transaction, index: 3, new[] { Input.Undefined() });
        transaction.Commit();
        undoManager.Undo();
        var result = undoManager.Redo();

        // Assert
        Assert.That(array.Length, Is.EqualTo(expected: 4));
        Assert.That(result, Is.True);

        // Act (remove, undo, and redo)
        transaction = doc.WriteTransaction();
        array.RemoveRange(transaction, index: 1, length: 2);
        transaction.Commit();
        undoManager.Undo();
        result = undoManager.Redo();

        // Assert
        Assert.That(array.Length, Is.EqualTo(expected: 2));
        Assert.That(result, Is.True);
    }

    [Test]
    public void RedoAddingAndUpdatingAndRemovingContentOnMap()
    {
        // Arrange
        var doc = new Doc();
        var map = doc.Map("map");
        var undoManager = new UndoManager(doc, map, new UndoManagerOptions { CaptureTimeoutMilliseconds = 0 });
        var transaction = doc.WriteTransaction();
        map.Insert(transaction, "value-1", Input.Long(value: 2469L));
        map.Insert(transaction, "value-2", Input.String("Lucas"));
        map.Insert(transaction, "value-3", Input.Boolean(value: true));
        transaction.Commit();

        // Act (add, undo, and redo)
        transaction = doc.WriteTransaction();
        map.Insert(transaction, "value-4", Input.Double(value: 4.20));
        transaction.Commit();
        undoManager.Undo();
        var result = undoManager.Redo();

        transaction = doc.ReadTransaction();
        var length = map.Length(transaction);
        transaction.Commit();

        // Assert
        Assert.That(length, Is.EqualTo(expected: 4));
        Assert.That(result, Is.True);

        // Act (replace, undo, and redo)
        transaction = doc.WriteTransaction();
        map.Insert(transaction, "value-2", Input.Undefined());
        transaction.Commit();
        undoManager.Undo();
        result = undoManager.Redo();

        transaction = doc.ReadTransaction();
        length = map.Length(transaction);
        var value2 = map.Get(transaction, "value-2");
        transaction.Commit();

        // Assert
        Assert.That(length, Is.EqualTo(expected: 4));
        Assert.That(value2.Tag, Is.EqualTo(OutputTage.Undefined));
        Assert.That(result, Is.True);

        // Act (remove, undo, and redo)
        transaction = doc.WriteTransaction();
        map.Remove(transaction, "value-1");
        transaction.Commit();
        undoManager.Undo();
        result = undoManager.Redo();

        transaction = doc.ReadTransaction();
        length = map.Length(transaction);
        var value1 = map.Get(transaction, "value-1");
        transaction.Commit();

        // Assert
        Assert.That(length, Is.EqualTo(expected: 3));
        Assert.That(value1, Is.Null);
        Assert.That(result, Is.True);
    }

    [Test]
    public void RedoAddingAndUpdatingAndRemovingContentOnXmlText()
    {
        // Arrange
        var doc = new Doc();
        var xmlText = doc.XmlText("xml-text");
        var undoManager = new UndoManager(doc, xmlText, new UndoManagerOptions { CaptureTimeoutMilliseconds = 0 });
        var transaction = doc.WriteTransaction();
        xmlText.Insert(transaction, index: 0, "Lucas");
        xmlText.InsertAttribute(transaction, "bold", "true");
        xmlText.InsertEmbed(transaction, index: 3, Input.Boolean(value: true));
        transaction.Commit();

        // Act (add text, attribute, and embed, undo, and redo)
        transaction = doc.WriteTransaction();
        xmlText.Insert(transaction, index: 9, " Viana");
        xmlText.InsertAttribute(transaction, "italic", "false");
        xmlText.InsertEmbed(transaction, index: 7, Input.Bytes(new byte[] { 2, 4, 6, 9 }));
        transaction.Commit();
        undoManager.Undo();
        var result = undoManager.Redo();

        transaction = doc.ReadTransaction();
        var text = xmlText.String(transaction);
        var attributes = xmlText.Iterate(transaction).ToArray();
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("Luctrueas 0x02040609Viana"));
        Assert.That(attributes.Length, Is.EqualTo(expected: 2));
        Assert.That(attributes.First(x => x.Name == "bold").Value, Is.EqualTo("true"));
        Assert.That(attributes.First(x => x.Name == "italic").Value, Is.EqualTo("false"));
        Assert.That(result, Is.True);

        // Act (remove text, attribute, and embed, undo, and redo)
        transaction = doc.WriteTransaction();
        xmlText.RemoveRange(transaction, index: 0, length: 7);
        xmlText.RemoveAttribute(transaction, "bold");
        transaction.Commit();
        undoManager.Undo();
        result = undoManager.Redo();

        transaction = doc.ReadTransaction();
        text = xmlText.String(transaction);
        attributes = xmlText.Iterate(transaction).ToArray();
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("0x02040609Viana"));
        Assert.That(attributes.Length, Is.EqualTo(expected: 1));
        Assert.That(attributes.First(x => x.Name == "italic").Value, Is.EqualTo("false"));
        Assert.That(result, Is.True);
    }

    [Test]
    public void RedoAddingAndUpdatingAndRemovingContentOnXmlElement()
    {
        // Arrange
        var doc = new Doc();
        var xmlElement = doc.XmlElement("xml-element");
        var undoManager = new UndoManager(doc, xmlElement, new UndoManagerOptions { CaptureTimeoutMilliseconds = 0 });
        var transaction = doc.WriteTransaction();
        xmlElement.InsertText(transaction, index: 0);
        xmlElement.InsertAttribute(transaction, "bold", "true");
        xmlElement.InsertElement(transaction, index: 1, "color");
        transaction.Commit();

        // Act (add text, attribute, and element, undo, and redo)
        transaction = doc.WriteTransaction();
        xmlElement.InsertText(transaction, index: 2);
        xmlElement.InsertAttribute(transaction, "italic", "false");
        xmlElement.InsertElement(transaction, index: 3, "size");
        transaction.Commit();
        undoManager.Undo();
        var result = undoManager.Redo();

        transaction = doc.ReadTransaction();
        var length = xmlElement.ChildLength(transaction);
        var attributes = xmlElement.Iterate(transaction).ToArray();
        transaction.Commit();

        // Assert
        Assert.That(length, Is.EqualTo(expected: 4));
        Assert.That(attributes.Length, Is.EqualTo(expected: 2));
        Assert.That(attributes.First(x => x.Name == "bold").Value, Is.EqualTo("true"));
        Assert.That(attributes.First(x => x.Name == "italic").Value, Is.EqualTo("false"));
        Assert.That(result, Is.True);

        // Act (remove text, attribute, and element, undo, and redo)
        transaction = doc.WriteTransaction();
        xmlElement.RemoveRange(transaction, index: 0, length: 2);
        xmlElement.RemoveAttribute(transaction, "italic");
        transaction.Commit();
        undoManager.Undo();
        result = undoManager.Redo();

        transaction = doc.ReadTransaction();
        length = xmlElement.ChildLength(transaction);
        attributes = xmlElement.Iterate(transaction).ToArray();
        transaction.Commit();

        // Assert
        Assert.That(length, Is.EqualTo(expected: 2));
        Assert.That(attributes.Length, Is.EqualTo(expected: 1));
        Assert.That(attributes.First(x => x.Name == "bold").Value, Is.EqualTo("true"));
        Assert.That(result, Is.True);
    }
}
