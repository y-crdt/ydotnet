using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.UndoManagers;

namespace YDotNet.Tests.Unit.UndoManagers;

public class CanUndoTests
{
    [Test]
    public void CanNotUndoInitially()
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
    public void CanUndoAfterApplyingChanges()
    {
        // Arrange
        var doc = new Doc();
        var text = doc.Text("text");
        var undoManager = new UndoManager(doc, text, new UndoManagerOptions { CaptureTimeoutMilliseconds = 0 });

        // Act
        var transaction = doc.WriteTransaction();
        text.Insert(transaction, index: 0, "Lucas");
        transaction.Commit();
        var canUndo = undoManager.CanUndo();

        // Assert
        Assert.That(canUndo, Is.True);

        // Act
        var undo = undoManager.Undo();
        canUndo = undoManager.CanUndo();

        // Assert
        Assert.That(undo, Is.True);
        Assert.That(canUndo, Is.False);
    }
}
