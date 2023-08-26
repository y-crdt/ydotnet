using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.UndoManagers;

namespace YDotNet.Tests.Unit.UndoManagers;

public class NewTests
{
    [Test]
    public void CreateWithOptions()
    {
        // Arrange
        var doc = new Doc();
        var text = doc.Text("text");

        // Act
        var undoManager = new UndoManager(
            doc, text, new UndoManagerOptions
            {
                CaptureTimeoutMilliseconds = 1000
            });

        // Assert
        Assert.That(undoManager.Handle, Is.GreaterThan(nint.Zero));
    }

    [Test]
    public void CreateWithoutOptions()
    {
        // Arrange
        var doc = new Doc();
        var text = doc.Text("text");

        // Act
        var undoManager = new UndoManager(doc, text);

        // Assert
        Assert.That(undoManager.Handle, Is.GreaterThan(nint.Zero));
    }
}
