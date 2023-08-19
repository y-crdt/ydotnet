using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;
using YDotNet.Document.Types.XmlTexts;

namespace YDotNet.Tests.Unit.XmlTexts;

public class InsertEmbedTests
{
    [Test]
    public void InsertBooleanEmbed()
    {
        // Arrange
        var (doc, xmlText) = ArrangeXmlText();

        // Act
        var transaction = doc.WriteTransaction();
        xmlText.InsertEmbed(transaction, index: 3, Input.Boolean(value: true));
        var text = xmlText.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("Luctrueas"));
    }

    [Test]
    public void InsertDoubleEmbed()
    {
        // Arrange
        var (doc, xmlText) = ArrangeXmlText();

        // Act
        var transaction = doc.WriteTransaction();
        xmlText.InsertEmbed(transaction, index: 3, Input.Double(value: 24.69));
        var text = xmlText.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("Luc24.69as"));
    }

    [Test]
    public void InsertLongEmbed()
    {
        // Arrange
        var (doc, xmlText) = ArrangeXmlText();

        // Act
        var transaction = doc.WriteTransaction();
        xmlText.InsertEmbed(transaction, index: 3, Input.Long(value: 2469));

        var text = xmlText.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("Luc2469as"));
    }

    [Test]
    public void InsertStringEmbed()
    {
        // Arrange
        var (doc, xmlText) = ArrangeXmlText();

        // Act
        var transaction = doc.WriteTransaction();
        xmlText.InsertEmbed(transaction, index: 3, Input.String("Between"));

        var text = xmlText.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("LucBetweenas"));
    }

    [Test]
    public void InsertBytesEmbed()
    {
        // Arrange
        var (doc, xmlText) = ArrangeXmlText();

        // Act
        var transaction = doc.WriteTransaction();
        xmlText.InsertEmbed(transaction, index: 3, Input.Bytes(new byte[] { 2, 4, 6, 9 }));

        var text = xmlText.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("Luc0x02040609as"));
    }

    [Test]
    public void InsertCollectionEmbed()
    {
        // Arrange
        var (doc, xmlText) = ArrangeXmlText();

        // Act
        var transaction = doc.WriteTransaction();
        xmlText.InsertEmbed(
            transaction, index: 3, Input.Collection(
                new[]
                {
                    Input.Boolean(value: true),
                    Input.Boolean(value: false)
                }));

        var text = xmlText.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("Luc[true, false]as"));
    }

    [Test]
    public void InsertObjectEmbed()
    {
        // Arrange
        var (doc, xmlText) = ArrangeXmlText();

        // Act
        var transaction = doc.WriteTransaction();
        xmlText.InsertEmbed(
            transaction, index: 3, Input.Object(
                new Dictionary<string, Input>
                {
                    { "italics", Input.Boolean(value: true) }
                }));

        var text = xmlText.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("Luc{italics: true}as"));
    }

    [Test]
    public void InsertNullEmbed()
    {
        // Arrange
        var (doc, xmlText) = ArrangeXmlText();

        // Act
        var transaction = doc.WriteTransaction();
        xmlText.InsertEmbed(transaction, index: 3, Input.Null());

        var text = xmlText.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("Lucnullas"));
    }

    [Test]
    public void InsertUndefinedEmbed()
    {
        // Arrange
        var (doc, xmlText) = ArrangeXmlText();

        // Act
        var transaction = doc.WriteTransaction();
        xmlText.InsertEmbed(transaction, index: 3, Input.Undefined());

        var text = xmlText.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("Lucundefinedas"));
    }

    [Test]
    public void InsertBooleanEmbedWithAttributes()
    {
        // Arrange
        var (doc, xmlText) = ArrangeXmlText();
        var attributes = new Dictionary<string, Input>
        {
            { "bold", Input.Boolean(value: true) },
            {
                "color", Input.Object(
                    new Dictionary<string, Input>
                    {
                        { "hex", Input.String("#FEAE46") }
                    })
            }
        };

        // Act
        var transaction = doc.WriteTransaction();
        xmlText.InsertEmbed(transaction, index: 3, Input.Boolean(value: true), Input.Object(attributes));
        var text = xmlText.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("Luc<bold><color hex=\"#FEAE46\">true</color></bold>as"));
    }

    private (Doc, XmlText) ArrangeXmlText()
    {
        var doc = new Doc();
        var xmlText = doc.XmlText("xml-text");

        var transaction = doc.WriteTransaction();
        xmlText.Insert(transaction, index: 0, "Lucas");
        transaction.Commit();

        return (doc, xmlText);
    }
}
