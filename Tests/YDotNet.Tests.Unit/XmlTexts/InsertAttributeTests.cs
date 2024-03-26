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
        var value = xmlText.GetAttribute(transaction, string.Empty);
        transaction.Commit();

        // Assert
        Assert.That(value, Is.EqualTo(string.Empty));
    }

    [Test]
    public void InsertAttributeWithEmptyNameAndFilledValue()
    {
        // Arrange
        var (doc, xmlText) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        xmlText.InsertAttribute(transaction, string.Empty, "🔭");
        var value = xmlText.GetAttribute(transaction, string.Empty);
        transaction.Commit();

        // Assert
        Assert.That(value, Is.EqualTo("🔭"));
    }

    [Test]
    public void InsertAttributeWithFilledNameAndEmptyValue()
    {
        // Arrange
        var (doc, xmlText) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        xmlText.InsertAttribute(transaction, "telescope", string.Empty);
        var value = xmlText.GetAttribute(transaction, "telescope");
        transaction.Commit();

        // Assert
        Assert.That(value, Is.EqualTo(string.Empty));
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
        var telescopeAttribute = xmlText.GetAttribute(transaction, "telescope");
        var mirrorAttribute = xmlText.GetAttribute(transaction, "mirror");
        transaction.Commit();

        // Assert
        Assert.That(telescopeAttribute, Is.EqualTo("🔭"));
        Assert.That(mirrorAttribute, Is.EqualTo("🪞"));
    }

    [Test]
    public void InsertAttributeWithTheSameNameReplacesIt()
    {
        // Arrange
        var (doc, xmlText) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        xmlText.InsertAttribute(transaction, "number", "1️⃣");
        var value = xmlText.GetAttribute(transaction, "number");
        transaction.Commit();

        // Assert
        Assert.That(value, Is.EqualTo("1️⃣"));

        // Act
        transaction = doc.WriteTransaction();
        xmlText.InsertAttribute(transaction, "number", "2️⃣");
        value = xmlText.GetAttribute(transaction, "number");
        transaction.Commit();

        // Assert
        Assert.That(value, Is.EqualTo("2️⃣"));
    }

    private static (Doc, XmlText) ArrangeDoc()
    {
        var doc = new Doc();
        var xmlFragment = doc.XmlFragment("xml-fragment");

        var transaction = doc.WriteTransaction();
        var xmlText = xmlFragment.InsertText(transaction, index: 0);
        xmlText.Insert(transaction, index: 0, "saturn-🪐");
        transaction.Commit();

        return (doc, xmlText);
    }
}
