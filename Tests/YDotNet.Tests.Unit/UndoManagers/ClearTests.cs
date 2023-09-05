using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.UndoManagers;

namespace YDotNet.Tests.Unit.UndoManagers;

public class ClearTests
{
    [Test]
    public void ClearDisablesUndoing()
    {
        // Arrange
        var doc = new Doc();
        var text = doc.Text("text");
        var undoManager = new UndoManager(doc, text, new UndoManagerOptions { CaptureTimeoutMilliseconds = 0 });

        var transaction = doc.WriteTransaction();
        text.Insert(transaction, index: 0, "Lucas");
        transaction.Commit();

        // Act
        var canUndo1 = undoManager.CanUndo();
        var clear = undoManager.Clear();
        var canUndo2 = undoManager.CanUndo();

        // Assert
        Assert.That(canUndo1, Is.True);
        Assert.That(clear, Is.True);
        Assert.That(canUndo2, Is.False);

        // Act
        undoManager.Undo();
        transaction = doc.ReadTransaction();
        var value = text.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(value, Is.EqualTo("Lucas"));
    }

    [Test]
    public void ClearDisablesRedoing()
    {
        // Arrange
        var doc = new Doc();
        var text = doc.Text("text");
        var undoManager = new UndoManager(doc, text, new UndoManagerOptions { CaptureTimeoutMilliseconds = 0 });

        var transaction = doc.WriteTransaction();
        text.Insert(transaction, index: 0, "Lucas");
        transaction.Commit();

        transaction = doc.WriteTransaction();
        text.Insert(transaction, index: 5, " Viana");
        transaction.Commit();

        // Act
        undoManager.Undo();
        var canRedo1 = undoManager.CanRedo();
        var clear = undoManager.Clear();
        var canRedo2 = undoManager.CanRedo();

        // Assert
        Assert.That(canRedo1, Is.True);
        Assert.That(clear, Is.True);
        Assert.That(canRedo2, Is.False);

        // Act
        undoManager.Redo();
        transaction = doc.ReadTransaction();
        var value = text.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(value, Is.EqualTo("Lucas"));
    }
}
