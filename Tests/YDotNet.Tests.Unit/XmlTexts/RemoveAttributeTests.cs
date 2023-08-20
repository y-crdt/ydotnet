using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Types.XmlTexts;

namespace YDotNet.Tests.Unit.XmlTexts;

public class RemoveAttributeTests
{
    [Test]
    public void RemoveAttributeThatDoesNotExist()
    {
        // Arrange
        var (doc, xmlText) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        xmlText.RemoveAttribute(transaction, "size");
        var emptyAttribute = xmlText.GetAttribute(transaction, "empty");
        var numberAttribute = xmlText.GetAttribute(transaction, "number");
        transaction.Commit();

        // Assert
        Assert.That(emptyAttribute, Is.EqualTo(string.Empty));
        Assert.That(numberAttribute, Is.EqualTo("7️⃣"));
    }

    [Test]
    public void RemoveAttributeThatExists()
    {
        // Arrange
        var (doc, xmlText) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        xmlText.RemoveAttribute(transaction, "number");
        var emptyAttribute = xmlText.GetAttribute(transaction, "empty");
        var numberAttribute = xmlText.GetAttribute(transaction, "number");
        transaction.Commit();

        // Assert
        Assert.That(emptyAttribute, Is.EqualTo(string.Empty));
        Assert.That(numberAttribute, Is.Null);
    }

    private (Doc, XmlText) ArrangeDoc()
    {
        var doc = new Doc();
        var xmlText = doc.XmlText("xml-text");

        var transaction = doc.WriteTransaction();
        xmlText.InsertAttribute(transaction, "empty", string.Empty);
        xmlText.InsertAttribute(transaction, "number", "7️⃣");
        transaction.Commit();

        return (doc, xmlText);
    }
}
