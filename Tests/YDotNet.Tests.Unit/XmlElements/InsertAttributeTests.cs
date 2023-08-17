using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;
using YDotNet.Document.Types.XmlElements;

namespace YDotNet.Tests.Unit.XmlElements;

public class InsertAttributeTests
{
    [Test]
    public void InsertAttributeWithEmptyNameAndValue()
    {
        // Arrange
        var (doc, xmlElement) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        xmlElement.InsertAttribute(transaction, string.Empty, string.Empty);
        transaction.Commit();

        // Assert
        Assert.That(xmlElement.String, Is.EqualTo("<link =\"\"></link>"));
    }

    [Test]
    public void InsertAttributeWithEmptyNameAndFilledValue()
    {
        // Arrange
        var (doc, xmlElement) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        xmlElement.InsertAttribute(transaction, string.Empty, "https://lsviana.github.io/");
        transaction.Commit();

        // Assert
        Assert.That(xmlElement.String, Is.EqualTo("<link =\"https://lsviana.github.io/\"></link>"));
    }

    [Test]
    public void InsertAttributeWithFilledNameAndEmptyValue()
    {
        // Arrange
        var (doc, xmlElement) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        xmlElement.InsertAttribute(transaction, "data-ydotnet", string.Empty);
        transaction.Commit();

        // Assert
        Assert.That(xmlElement.String, Is.EqualTo("<link data-ydotnet=\"\"></link>"));
    }

    [Test]
    public void InsertAttributeWithFilledNameAndValue()
    {
        // Arrange
        var (doc, xmlElement) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        xmlElement.InsertAttribute(transaction, "href", "https://lsviana.github.io/");
        transaction.Commit();

        // Assert
        Assert.That(xmlElement.String, Is.EqualTo("<link href=\"https://lsviana.github.io/\"></link>"));
    }

    [Test]
    public void InsertMultipleAttributes()
    {
        // Arrange
        var (doc, xmlElement) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        xmlElement.InsertAttribute(transaction, "href", "https://lsviana.github.io/");
        xmlElement.InsertAttribute(transaction, "rel", "preload");
        xmlElement.InsertAttribute(transaction, "as", "document");
        xmlElement.InsertAttribute(transaction, "🛫", "🛬");
        transaction.Commit();

        // Assert
        Assert.That(xmlElement.String, Contains.Substring("<link"));
        Assert.That(xmlElement.String, Contains.Substring("href=\"https://lsviana.github.io/\""));
        Assert.That(xmlElement.String, Contains.Substring("rel=\"preload\""));
        Assert.That(xmlElement.String, Contains.Substring("as=\"document\""));
        Assert.That(xmlElement.String, Contains.Substring("🛫=\"🛬\""));
        Assert.That(xmlElement.String, Contains.Substring("></link>"));
    }

    [Test]
    public void InsertAttributeWithTheSameNameReplacesIt()
    {
        // Arrange
        var (doc, xmlElement) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        xmlElement.InsertAttribute(transaction, "as", "document");
        transaction.Commit();

        // Assert
        Assert.That(xmlElement.String, Is.EqualTo("<link as=\"document\"></link>"));

        // Act
        transaction = doc.WriteTransaction();
        xmlElement.InsertAttribute(transaction, "as", "stylesheet");
        transaction.Commit();

        // Assert
        Assert.That(xmlElement.String, Is.EqualTo("<link as=\"stylesheet\"></link>"));
    }

    private static (Doc, XmlElement) ArrangeDoc()
    {
        var doc = new Doc();
        var array = doc.Array("array");

        var transaction = doc.WriteTransaction();
        array.InsertRange(transaction, index: 0, new[] { Input.XmlElement("link") });
        var xmlElement = array.Get(transaction, index: 0).XmlElement;
        transaction.Commit();

        return (doc, xmlElement);
    }
}
