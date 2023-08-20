using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;
using YDotNet.Document.Types.XmlTexts;

namespace YDotNet.Tests.Unit.XmlTexts;

public class FormatTests
{
    [Test]
    public void FormatsTextAtBeginning()
    {
        // Arrange
        var (doc, xmlText) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        xmlText.Format(
            transaction, index: 0, length: 2, Input.Object(
                new Dictionary<string, Input>
                {
                    { "bold", Input.Boolean(value: true) },
                    {
                        "color", Input.Object(
                            new Dictionary<string, Input>
                            {
                                { "hex", Input.String("#FEAE46") },
                                { "alpha", Input.Double(value: 0.5) }
                            })
                    }
                }));
        var text = xmlText.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(
            text, Is.AnyOf(
                "<bold><color hex=\"#FEAE46\" alpha=\"0.5\">Lu</color></bold>cas",
                "<bold><color alpha=\"0.5\" hex=\"#FEAE46\">Lu</color></bold>cas"));
    }

    [Test]
    public void FormatsTextAtMiddle()
    {
        // Arrange
        var (doc, xmlText) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        xmlText.Format(
            transaction, index: 2, length: 2, Input.Object(
                new Dictionary<string, Input>
                {
                    { "italic", Input.Boolean(value: true) }
                }));
        var text = xmlText.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("Lu<italic>ca</italic>s"));
    }

    [Test]
    public void FormatsTextAtEnding()
    {
        // Arrange
        var (doc, xmlText) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        xmlText.Format(
            transaction, index: 3, length: 5, Input.Object(
                new Dictionary<string, Input>
                {
                    { "sub", Input.Boolean(value: true) }
                }));
        var text = xmlText.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("Luc<sub>as</sub>"));
    }

    private (Doc, XmlText) ArrangeDoc()
    {
        var doc = new Doc();
        var xmlText = doc.XmlText("xml-text");

        var transaction = doc.WriteTransaction();
        xmlText.Insert(transaction, index: 0, "Lucas");
        transaction.Commit();

        return (doc, xmlText);
    }
}
