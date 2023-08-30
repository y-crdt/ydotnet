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
    [Ignore("Waiting to be implemented.")]
    public void RedoAddingAndUpdatingAndRemovingContentOnArray()
    {
    }

    [Test]
    [Ignore("Waiting to be implemented.")]
    public void RedoAddingAndUpdatingAndRemovingContentOnMap()
    {
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
