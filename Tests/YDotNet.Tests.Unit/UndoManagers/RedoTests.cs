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
        Assert.That(result, Is.True);

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
        Assert.That(value2.Undefined, Is.True);
        Assert.That(value2.String, Is.Null);
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
    [Ignore("Waiting to be implemented.")]
    public void RedoAddingAndUpdatingAndRemovingContentOnXmlText()
    {
    }

    [Test]
    [Ignore("Waiting to be implemented.")]
    public void RedoAddingAndUpdatingAndRemovingContentOnXmlElement()
    {
    }
}
