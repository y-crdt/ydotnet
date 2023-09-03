using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.UndoManagers;

namespace YDotNet.Tests.Unit.UndoManagers;

public class RemoveOriginTests
{
    [Test]
    public void TracksAssignedOriginUntilRemoval()
    {
        // Arrange
        var doc = new Doc();
        var text = doc.Text("text");
        var undoManager = new UndoManager(doc, text, new UndoManagerOptions { CaptureTimeoutMilliseconds = 0 });
        undoManager.AddOrigin(new byte[] { 0 });

        var called = 0;
        undoManager.ObserveAdded(_ => called++);

        // Act
        var transaction = doc.WriteTransaction(new byte[] { 0 });
        text.Insert(transaction, index: 0, "Lucas");
        transaction.Commit();

        // Assert
        Assert.That(called, Is.EqualTo(expected: 1));

        // Act
        undoManager.RemoveOrigin(new byte[] { 0 });
        transaction = doc.WriteTransaction(new byte[] { 0 });
        text.Insert(transaction, index: 5, " Viana");
        transaction.Commit();

        // Assert
        Assert.That(called, Is.EqualTo(expected: 1));

        // Act
        transaction = doc.WriteTransaction();
        text.Insert(transaction, index: 5, " Viana");
        transaction.Commit();

        // Assert
        Assert.That(called, Is.EqualTo(expected: 2));
    }
}
