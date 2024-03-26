using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;
using YDotNet.Document.UndoManagers;

namespace YDotNet.Tests.Unit.UndoManagers;

public class UndoTests
{
    [Test]
    [Ignore("Waiting for fix on yffi.")]
    public void ReturnsFalseWhenNoChangesApplied()
    {
        // Arrange
        var doc = new Doc();
        var text = doc.Text("text");
        var undoManager = new UndoManager(doc, text, new UndoManagerOptions { CaptureTimeoutMilliseconds = 0 });

        // Act
        var result = undoManager.Undo();

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void UndoAddingAndUpdatingAndRemovingContentOnText()
    {
        // Arrange
        var doc = new Doc();
        var text = doc.Text("text");
        var undoManager = new UndoManager(doc, text, new UndoManagerOptions { CaptureTimeoutMilliseconds = 0 });
        var transaction = doc.WriteTransaction();
        text.Insert(transaction, index: 0, "Lucas");
        transaction.Commit();

        // Act (add text and undo)
        transaction = doc.WriteTransaction();
        text.Insert(transaction, index: 0, " Viana");
        transaction.Commit();
        var result = undoManager.Undo();

        transaction = doc.ReadTransaction();
        var value = text.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(value, Is.EqualTo("Lucas"));
        Assert.That(result, Is.True);

        // Act (add embed and undo)
        transaction = doc.WriteTransaction();
        text.InsertEmbed(
            transaction, index: 3, Input.Boolean(value: true), Input.Object(
                new Dictionary<string, Input>
                {
                    { "bold", Input.Boolean(value: true) }
                }));
        transaction.Commit();
        result = undoManager.Undo();

        transaction = doc.ReadTransaction();
        var chunks = text.Chunks(transaction).ToArray();
        transaction.Commit();

        // Assert
        Assert.That(chunks.Length, Is.EqualTo(expected: 1));
        Assert.That(result, Is.True);

        // Act (remove and undo)
        transaction = doc.WriteTransaction();
        text.RemoveRange(transaction, index: 2, length: 2);
        transaction.Commit();
        result = undoManager.Undo();

        transaction = doc.ReadTransaction();
        value = text.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(value, Is.EqualTo("Lucas"));
        Assert.That(result, Is.True);
    }

    [Test]
    [Ignore("Waiting for fix on yffi.")]
    public void UndoAddingAndUpdatingAndRemovingContentOnArray()
    {
        // Arrange
        var doc = new Doc();
        var array = doc.Array("array");
        var undoManager = new UndoManager(doc, array, new UndoManagerOptions { CaptureTimeoutMilliseconds = 0 });
        var transaction = doc.WriteTransaction();
        array.InsertRange(
            transaction,
            index: 0, Input.Boolean(value: true), Input.Long(value: 2469L), Input.String("Lucas"));
        transaction.Commit();

        // Act (add and undo)
        transaction = doc.WriteTransaction();
        array.InsertRange(transaction, index: 3, Input.Undefined());
        transaction.Commit();
        var result = undoManager.Undo();

        // Assert
        Assert.That(array.Length, Is.EqualTo(expected: 3));
        Assert.That(result, Is.True);

        // Act (remove and undo)
        transaction = doc.WriteTransaction();
        array.RemoveRange(transaction, index: 1, length: 2);
        transaction.Commit();
        result = undoManager.Undo();

        // Assert

        // TODO [LSViana] Check with the team why the amount of items isn't 3.
        Assert.That(array.Length, Is.EqualTo(expected: 3));
        Assert.That(result, Is.True);
    }

    [Test]
    public void UndoAddingAndUpdatingAndRemovingContentOnMap()
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

        // Act (add and undo)
        transaction = doc.WriteTransaction();
        map.Insert(transaction, "value-4", Input.Double(value: 4.20));
        transaction.Commit();
        var result = undoManager.Undo();

        transaction = doc.ReadTransaction();
        var length = map.Length(transaction);
        transaction.Commit();

        // Assert
        Assert.That(length, Is.EqualTo(expected: 3));
        Assert.That(result, Is.True);

        // Act (replace and undo)
        transaction = doc.WriteTransaction();
        map.Insert(transaction, "value-2", Input.Undefined());
        transaction.Commit();
        result = undoManager.Undo();

        transaction = doc.ReadTransaction();
        length = map.Length(transaction);
        var value2 = map.Get(transaction, "value-2");
        transaction.Commit();

        // Assert
        Assert.That(length, Is.EqualTo(expected: 3));
        Assert.That(value2.String, Is.EqualTo("Lucas"));
        Assert.That(result, Is.True);

        // Act (remove and undo)
        transaction = doc.WriteTransaction();
        map.Remove(transaction, "value-1");
        transaction.Commit();
        result = undoManager.Undo();

        transaction = doc.ReadTransaction();
        length = map.Length(transaction);
        var value1 = map.Get(transaction, "value-1");
        transaction.Commit();

        // Assert
        Assert.That(length, Is.EqualTo(expected: 3));
        Assert.That(value1.Long, Is.EqualTo(expected: 2469L));
        Assert.That(result, Is.True);
    }

    [Test]
    public void UndoAddingAndUpdatingAndRemovingContentOnXmlText()
    {
        // Arrange
        var doc = new Doc();
        var xmlFragment = doc.XmlFragment("xml-fragment");

        var transaction = doc.WriteTransaction();
        var xmlText = xmlFragment.InsertText(transaction, index: 0);
        transaction.Commit();

        var undoManager = new UndoManager(doc, xmlText, new UndoManagerOptions { CaptureTimeoutMilliseconds = 0 });
        transaction = doc.WriteTransaction();
        xmlText.Insert(transaction, index: 0, "Lucas");
        xmlText.InsertAttribute(transaction, "bold", "true");
        xmlText.InsertEmbed(transaction, index: 3, Input.Boolean(value: true));
        transaction.Commit();

        // Act (add text, attribute, and embed and undo)
        transaction = doc.WriteTransaction();
        xmlText.Insert(transaction, index: 9, " Viana");
        xmlText.InsertAttribute(transaction, "italic", "false");
        xmlText.InsertEmbed(transaction, index: 7, Input.Bytes(new byte[] { 2, 4, 6, 9 }));
        transaction.Commit();
        var result = undoManager.Undo();

        transaction = doc.ReadTransaction();
        var text = xmlText.String(transaction);
        var attribute = xmlText.GetAttribute(transaction, "bold");
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("Luctrueas"));
        Assert.That(attribute, Is.EqualTo("true"));
        Assert.That(result, Is.True);

        // Act (remove text, attribute, and embed and undo)
        transaction = doc.WriteTransaction();
        xmlText.RemoveRange(transaction, index: 0, length: 6);
        xmlText.RemoveAttribute(transaction, "bold");
        transaction.Commit();
        result = undoManager.Undo();

        transaction = doc.ReadTransaction();
        text = xmlText.String(transaction);
        attribute = xmlText.GetAttribute(transaction, "bold");
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("Luctrueas"));
        Assert.That(attribute, Is.EqualTo("true"));
        Assert.That(result, Is.True);
    }

    [Test]
    public void UndoAddingAndUpdatingAndRemovingContentOnXmlElement()
    {
        // Arrange
        var doc = new Doc();
        var xmlFragment = doc.XmlFragment("xml-fragment");

        var transaction = doc.WriteTransaction();
        var xmlElement = xmlFragment.InsertElement(transaction, index: 0, "xml-element");
        transaction.Commit();

        var undoManager = new UndoManager(doc, xmlElement, new UndoManagerOptions { CaptureTimeoutMilliseconds = 0 });
        transaction = doc.WriteTransaction();
        xmlElement.InsertText(transaction, index: 0);
        xmlElement.InsertAttribute(transaction, "bold", "true");
        xmlElement.InsertElement(transaction, index: 1, "color");
        transaction.Commit();

        // Act (add text, attribute, and element and undo)
        transaction = doc.WriteTransaction();
        xmlElement.InsertText(transaction, index: 2);
        xmlElement.InsertAttribute(transaction, "italic", "false");
        xmlElement.InsertElement(transaction, index: 3, "size");
        transaction.Commit();
        var result = undoManager.Undo();

        transaction = doc.ReadTransaction();
        var length = xmlElement.ChildLength(transaction);
        var attribute = xmlElement.GetAttribute(transaction, "bold");
        transaction.Commit();

        // Assert
        Assert.That(length, Is.EqualTo(expected: 2));
        Assert.That(attribute, Is.EqualTo("true"));
        Assert.That(result, Is.True);

        // Act (remove text, attribute, and element and undo)
        transaction = doc.WriteTransaction();
        xmlElement.RemoveRange(transaction, index: 0, length: 2);
        xmlElement.RemoveAttribute(transaction, "italic");
        transaction.Commit();
        result = undoManager.Undo();

        transaction = doc.ReadTransaction();
        length = xmlElement.ChildLength(transaction);
        attribute = xmlElement.GetAttribute(transaction, "bold");
        transaction.Commit();

        // Assert
        Assert.That(length, Is.EqualTo(expected: 2));
        Assert.That(attribute, Is.EqualTo("true"));
        Assert.That(result, Is.True);
    }
}
