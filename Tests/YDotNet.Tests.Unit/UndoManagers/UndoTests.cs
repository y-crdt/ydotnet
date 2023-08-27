using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;
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
    public void UndoAddingAndUpdatingAndRemovingContentOnText()
    {
        // Arrange
        var doc = new Doc();
        var text = doc.Text("text");
        var undoManager = new UndoManager(doc, text, new UndoManagerOptions { CaptureTimeoutMilliseconds = 0 });
        var transaction = doc.WriteTransaction();
        text.Insert(transaction, index: 0, "Lucas");
        transaction.Commit();

        // Act (add text and undo)
        transaction = doc.WriteTransaction();
        text.Insert(transaction, index: 0, " Viana");
        transaction.Commit();
        var result = undoManager.Undo();

        transaction = doc.ReadTransaction();
        var value = text.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(value, Is.EqualTo("Lucas"));
        Assert.That(result, Is.True);

        // Act (add embed and undo)
        transaction = doc.WriteTransaction();
        text.InsertEmbed(
            transaction, index: 3, Input.Boolean(value: true), Input.Object(
                new Dictionary<string, Input>
                {
                    { "bold", Input.Boolean(value: true) }
                }));
        transaction.Commit();
        result = undoManager.Undo();

        transaction = doc.ReadTransaction();
        var chunks = text.Chunks(transaction).ToArray();
        transaction.Commit();

        // Assert
        Assert.That(chunks.Length, Is.EqualTo(expected: 1));
        Assert.That(result, Is.True);

        // Act (remove and undo)
        transaction = doc.WriteTransaction();
        text.RemoveRange(transaction, index: 2, length: 2);
        transaction.Commit();
        result = undoManager.Undo();

        transaction = doc.ReadTransaction();
        value = text.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(value, Is.EqualTo("Lucas"));
        Assert.That(result, Is.True);
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
