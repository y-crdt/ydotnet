using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;
using YDotNet.Document.Types.XmlElements;

namespace YDotNet.Tests.Unit.XmlElements;

public class RemoveAttributeTests
{
    [Test]
    public void RemoveAttributeThatExistsWithEmptyNameWorks()
    {
        // Arrange
        var (doc, xmlElement) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        xmlElement.RemoveAttribute(transaction, string.Empty);
        var text = xmlElement.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("<link as=\"stylesheet\"></link>"));
    }

    [Test]
    public void RemoveAttributeThatExistsWithFilledNameWorks()
    {
        // Arrange
        var (doc, xmlElement) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        xmlElement.RemoveAttribute(transaction, "as");
        var text = xmlElement.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("<link =\"\"></link>"));
    }

    [Test]
    public void RemoveAttributeThatDoesNotExistWorks()
    {
        // Arrange
        var (doc, xmlElement) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        xmlElement.RemoveAttribute(transaction, "rel");
        var text = xmlElement.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(text, Contains.Substring("<link"));
        Assert.That(text, Contains.Substring("=\"\""));
        Assert.That(text, Contains.Substring("as=\"stylesheet\""));
        Assert.That(text, Contains.Substring("></link>"));
    }

    private (Doc, XmlElement) ArrangeDoc()
    {
        var doc = new Doc();
        var array = doc.Array("array-1");

        var transaction = doc.WriteTransaction();
        array.InsertRange(transaction, index: 0, new[] { Input.XmlElement("link") });

        var xmlElement = array.Get(transaction, index: 0).XmlElement;
        xmlElement.InsertAttribute(transaction, string.Empty, string.Empty);
        xmlElement.InsertAttribute(transaction, "as", "stylesheet");

        transaction.Commit();

        return (doc, xmlElement);
    }
}
