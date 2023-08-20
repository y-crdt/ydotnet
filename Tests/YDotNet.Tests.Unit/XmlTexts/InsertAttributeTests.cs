using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Types.XmlTexts;

namespace YDotNet.Tests.Unit.XmlTexts;

public class InsertAttributeTests
{
    [Test]
    public void InsertAttributeWithEmptyNameAndValue()
    {
        // Arrange
        var (doc, xmlText) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        xmlText.InsertAttribute(transaction, string.Empty, string.Empty);
        var text = xmlText.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("saturn-🪐"));

        // TODO [LSViana] Replace this assertion with XmlText.GetAttribute() for more reliable verification.
    }

    [Test]
    public void InsertAttributeWithEmptyNameAndFilledValue()
    {
        // Arrange
        var (doc, xmlText) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        xmlText.InsertAttribute(transaction, string.Empty, "🔭");
        var text = xmlText.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("saturn-🪐"));

        // TODO [LSViana] Replace this assertion with XmlText.GetAttribute() for more reliable verification.
    }

    [Test]
    public void InsertAttributeWithFilledNameAndEmptyValue()
    {
        // Arrange
        var (doc, xmlText) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        xmlText.InsertAttribute(transaction, "telescope", string.Empty);
        var text = xmlText.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("saturn-🪐"));

        // TODO [LSViana] Replace this assertion with XmlText.GetAttribute() for more reliable verification.
    }

    [Test]
    public void InsertMultipleAttributeWithFilledNameAndValue()
    {
        // Arrange
        var (doc, xmlText) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        xmlText.InsertAttribute(transaction, "telescope", "🔭");
        xmlText.InsertAttribute(transaction, "mirror", "🪞");
        var text = xmlText.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("saturn-🪐"));

        // TODO [LSViana] Replace this assertion with XmlText.GetAttribute() for more reliable verification.
    }

    [Test]
    public void InsertAttributeWithTheSameNameReplacesIt()
    {
        // Arrange
        var (doc, xmlText) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        xmlText.InsertAttribute(transaction, "number", "1️⃣");
        var text = xmlText.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("saturn-🪐"));

        // TODO [LSViana] Replace this assertion with XmlText.GetAttribute() for more reliable verification.

        // Act
        transaction = doc.WriteTransaction();
        xmlText.InsertAttribute(transaction, "number", "2️⃣");
        text = xmlText.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("saturn-🪐"));

        // TODO [LSViana] Replace this assertion with XmlText.GetAttribute() for more reliable verification.
    }

    private static (Doc, XmlText) ArrangeDoc()
    {
        var doc = new Doc();
        var xmlText = doc.XmlText("xml-text");

        var transaction = doc.WriteTransaction();
        xmlText.Insert(transaction, index: 0, "saturn-🪐");
        transaction.Commit();

        return (doc, xmlText);
    }
}
