using NUnit.Framework;
using YDotNet.Document;
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
    [Ignore("Waiting to be implemented.")]
    public void RedoAddingAndUpdatingAndRemovingContentOnText()
    {
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
