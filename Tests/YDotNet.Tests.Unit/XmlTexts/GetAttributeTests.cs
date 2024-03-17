using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Types.XmlTexts;

namespace YDotNet.Tests.Unit.XmlTexts;

public class GetAttributeTests
{
    [Test]
    public void GetAttributeThatDoesNotExistReturnsNull()
    {
        // Arrange
        var (doc, xmlText) = ArrangeDoc();

        // Act
        var transaction = doc.ReadTransaction();
        var value = xmlText.GetAttribute(transaction, "size");
        transaction.Commit();

        // Assert
        Assert.That(value, Is.Null);
    }

    [Test]
    public void GetAttributeThatExistsAndIsEmptyWorks()
    {
        // Arrange
        var (doc, xmlText) = ArrangeDoc();

        // Act
        var transaction = doc.ReadTransaction();
        var value = xmlText.GetAttribute(transaction, "empty");
        transaction.Commit();

        // Assert
        Assert.That(value, Is.EqualTo(string.Empty));
    }

    [Test]
    public void GetAttributeThatExistsAndIsFilledWorks()
    {
        // Arrange
        var (doc, xmlText) = ArrangeDoc();

        // Act
        var transaction = doc.ReadTransaction();
        var value = xmlText.GetAttribute(transaction, "number");
        transaction.Commit();

        // Assert
        Assert.That(value, Is.EqualTo("7️⃣"));
    }

    private (Doc, XmlText) ArrangeDoc()
    {
        var doc = new Doc();
        var xmlFragment = doc.XmlFragment("xml-fragment");

        var transaction = doc.WriteTransaction();
        var xmlText = xmlFragment.InsertText(transaction, index: 0);
        xmlText.InsertAttribute(transaction, "empty", string.Empty);
        xmlText.InsertAttribute(transaction, "number", "7️⃣");
        transaction.Commit();

        return (doc, xmlText);
    }
}
