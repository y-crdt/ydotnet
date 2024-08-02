using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;
using YDotNet.Document.UndoManagers;
using YDotNet.Document.UndoManagers.Events;

namespace YDotNet.Tests.Unit.UndoManagers;

public class AddScopeTests
{
    [Test]
    public void StartsTrackingAfterAddingScope()
    {
        // Arrange
        var doc = new Doc();
        var text = doc.Text("text");
        var array = doc.Array("array");
        var undoManager = new UndoManager(doc, text, new UndoManagerOptions { CaptureTimeoutMilliseconds = 0 });

        UndoEvent? undoEvent = null;
        undoManager.ObserveAdded(e => undoEvent = e);

        // Act
        undoEvent = null;
        var transaction = doc.WriteTransaction();
        array.InsertRange(transaction, index: 0, new[] { Input.Long(value: 2469L) });
        transaction.Commit();

        // Assert
        Assert.That(undoEvent, Is.Null);

        // Act
        undoEvent = null;
        undoManager.AddScope(array);
        transaction = doc.WriteTransaction();
        array.InsertRange(transaction, index: 1, new[] { Input.Boolean(value: false) });
        transaction.Commit();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
    }

    [Test]
    public void StartsTrackingAfterAddingScopeWithoutDefaultScope()
    {
        // Arrange
        var doc = new Doc();
        var array = doc.Array("array");
        var undoManager = new UndoManager(doc, new UndoManagerOptions { CaptureTimeoutMilliseconds = 0 });

        UndoEvent? undoEvent = null;
        undoManager.ObserveAdded(e => undoEvent = e);

        // Act
        undoEvent = null;
        var transaction = doc.WriteTransaction();
        array.InsertRange(transaction, index: 0, new[] { Input.Long(value: 2469L) });
        transaction.Commit();

        // Assert
        Assert.That(undoEvent, Is.Null);

        // Act
        undoEvent = null;
        undoManager.AddScope(array);
        transaction = doc.WriteTransaction();
        array.InsertRange(transaction, index: 1, new[] { Input.Boolean(value: false) });
        transaction.Commit();

        // Assert
        Assert.That(undoEvent, Is.Not.Null);
    }
}
