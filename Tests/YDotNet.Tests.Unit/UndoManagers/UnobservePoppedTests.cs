using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.UndoManagers;
using YDotNet.Document.UndoManagers.Events;

namespace YDotNet.Tests.Unit.UndoManagers;

public class UnobservePoppedTests
{
    [Test]
    public void TriggersWhenChangesUntilUnobserved()
    {
        // Arrange
        var doc = new Doc();
        var text = doc.Text("text");
        var undoManager = new UndoManager(doc, text, new UndoManagerOptions { CaptureTimeoutMilliseconds = 0 });

        UndoEvent? undoEvent = null;
        var subscription = undoManager.ObservePopped(e => undoEvent = e);

        // Act
        var transaction = doc.WriteTransaction();
        text.Insert(transaction, index: 0, "Lucas");
        transaction.Commit();
        undoManager.Undo();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);

        // Act
        undoEvent = null;
        subscription.Dispose();
        undoManager.Redo();

        // Assert
        Assert.That(undoEvent, Is.Null);
    }
}
