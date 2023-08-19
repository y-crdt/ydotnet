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
        var text = xmlElement.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("<link =\"\"></link>"));
    }

    [Test]
    public void InsertAttributeWithEmptyNameAndFilledValue()
    {
        // Arrange
        var (doc, xmlElement) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        xmlElement.InsertAttribute(transaction, string.Empty, "https://lsviana.github.io/");
        var text = xmlElement.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("<link =\"https://lsviana.github.io/\"></link>"));
    }

    [Test]
    public void InsertAttributeWithFilledNameAndEmptyValue()
    {
        // Arrange
        var (doc, xmlElement) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        xmlElement.InsertAttribute(transaction, "data-ydotnet", string.Empty);
        var text = xmlElement.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("<link data-ydotnet=\"\"></link>"));
    }

    [Test]
    public void InsertAttributeWithFilledNameAndValue()
    {
        // Arrange
        var (doc, xmlElement) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        xmlElement.InsertAttribute(transaction, "href", "https://lsviana.github.io/");
        var text = xmlElement.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("<link href=\"https://lsviana.github.io/\"></link>"));
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
        var text = xmlElement.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(text, Contains.Substring("<link"));
        Assert.That(text, Contains.Substring("href=\"https://lsviana.github.io/\""));
        Assert.That(text, Contains.Substring("rel=\"preload\""));
        Assert.That(text, Contains.Substring("as=\"document\""));
        Assert.That(text, Contains.Substring("🛫=\"🛬\""));
        Assert.That(text, Contains.Substring("></link>"));
    }

    [Test]
    public void InsertAttributeWithTheSameNameReplacesIt()
    {
        // Arrange
        var (doc, xmlElement) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        xmlElement.InsertAttribute(transaction, "as", "document");
        var text = xmlElement.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("<link as=\"document\"></link>"));

        // Act
        transaction = doc.WriteTransaction();
        xmlElement.InsertAttribute(transaction, "as", "stylesheet");
        text = xmlElement.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("<link as=\"stylesheet\"></link>"));
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
