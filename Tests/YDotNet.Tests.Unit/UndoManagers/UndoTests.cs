using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.UndoManagers;

namespace YDotNet.Tests.Unit.UndoManagers;

public class UndoTests
{
    [Test]
    public void ReturnsFalseWhenNoChangesApplied()
    {
        // Arrange
        var doc = new Doc();
        var text = doc.Text("text");
        var undoManager = new UndoManager(doc, text, new UndoManagerOptions { CaptureTimeoutMilliseconds = 0 });

        // Act
        var result = undoManager.Undo();

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    [Ignore("Waiting to be implemented.")]
    public void UndoAddingAndUpdatingAndRemovingContentOnText()
    {
    }

    [Test]
    [Ignore("Waiting to be implemented.")]
    public void UndoAddingAndUpdatingAndRemovingContentOnArray()
    {
    }

    [Test]
    [Ignore("Waiting to be implemented.")]
    public void UndoAddingAndUpdatingAndRemovingContentOnMap()
    {
    }

    [Test]
    [Ignore("Waiting to be implemented.")]
    public void UndoAddingAndUpdatingAndRemovingContentOnXmlText()
    {
    }

    [Test]
    [Ignore("Waiting to be implemented.")]
    public void UndoAddingAndUpdatingAndRemovingContentOnXmlElement()
    {
    }
}
