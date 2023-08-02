using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;
using YDotNet.Document.Types.Texts;

namespace YDotNet.Tests.Unit.Texts;

public class FormatTests
{
    [Test]
    public void FormatsTextAtBeginning()
    {
        // Arrange
        var (doc, text) = ArrangeText();

        // Act
        var transaction = doc.WriteTransaction();
        text.Format(
            transaction, index: 0, length: 2, Input.Object(
                new Dictionary<string, Input>
                {
                    { "bold", Input.Boolean(value: true) }
                }));
        transaction.Commit();

        // Assert
        // TODO Assert using `ytext_chunks` later.
    }

    [Test]
    public void FormatsTextAtMiddle()
    {
        // Arrange
        var (doc, text) = ArrangeText();

        // Act
        var transaction = doc.WriteTransaction();
        text.Format(
            transaction, index: 2, length: 2, Input.Object(
                new Dictionary<string, Input>
                {
                    { "bold", Input.Boolean(value: true) }
                }));
        transaction.Commit();

        // Assert
        // TODO Assert using `ytext_chunks` later.
    }

    [Test]
    public void FormatsTextAtEnding()
    {
        // Arrange
        var (doc, text) = ArrangeText();

        // Act
        var transaction = doc.WriteTransaction();
        text.Format(
            transaction, index: 3, length: 2, Input.Object(
                new Dictionary<string, Input>
                {
                    { "bold", Input.Boolean(value: true) }
                }));
        transaction.Commit();

        // Assert
        // TODO Assert using `ytext_chunks` later.
    }

    private (Doc, Text) ArrangeText()
    {
        var doc = new Doc();
        var text = doc.Text("value");

        var transaction = doc.WriteTransaction();
        text.Insert(transaction, index: 0, "Lucas");
        transaction.Commit();

        return (doc, text);
    }
}
