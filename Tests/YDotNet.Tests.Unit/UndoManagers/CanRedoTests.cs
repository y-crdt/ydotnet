using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.UndoManagers;

namespace YDotNet.Tests.Unit.UndoManagers;

public class CanRedoTests
{
    [Test]
    public void CanNotRedoInitially()
    {
        // Arrange
        var doc = new Doc();
        var text = doc.Text("text");
        var undoManager = new UndoManager(doc, text, new UndoManagerOptions { CaptureTimeoutMilliseconds = 0 });

        // Act
        var canUndo = undoManager.CanUndo();

        // Assert
        Assert.That(canUndo, Is.False);
    }

    [Test]
    public void CanRedoAfterUndoingChanges()
    {
        // Arrange
        var doc = new Doc();
        var text = doc.Text("text");
        var undoManager = new UndoManager(doc, text, new UndoManagerOptions { CaptureTimeoutMilliseconds = 0 });

        // Act
        var transaction = doc.WriteTransaction();
        text.Insert(transaction, index: 0, "Lucas");
        transaction.Commit();
        var canRedo = undoManager.CanRedo();

        // Assert
        Assert.That(canRedo, Is.False);

        // Act
        var undo = undoManager.Undo();
        canRedo = undoManager.CanRedo();

        // Assert
        Assert.That(undo, Is.True);
        Assert.That(canRedo, Is.True);
    }
}
