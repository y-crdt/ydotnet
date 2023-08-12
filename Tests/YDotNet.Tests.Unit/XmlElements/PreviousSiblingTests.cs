using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Types.XmlElements;

namespace YDotNet.Tests.Unit.XmlElements;

public class PreviousSiblingTests
{
    [Test]
    public void GetsPreviousSiblingAtBeginning()
    {
        // Arrange
        var (doc, xmlElement) = ArrangeDoc();

        // Act
        var transaction = doc.ReadTransaction();
        var targetXmlElement = xmlElement.Get(transaction, index: 0).XmlElement;
        var siblingElement = targetXmlElement.PreviousSibling(transaction);
        transaction.Commit();

        // Assert
        Assert.That(siblingElement, Is.Null);
    }

    [Test]
    public void GetsPreviousSiblingAtMiddle()
    {
        // Arrange
        var (doc, xmlElement) = ArrangeDoc();

        // Act
        var transaction = doc.ReadTransaction();
        var targetXmlElement = xmlElement.Get(transaction, index: 2).XmlElement;
        var siblingElement = targetXmlElement.PreviousSibling(transaction);
        transaction.Commit();

        // Assert
        Assert.That(siblingElement.XmlElement, Is.Null);
        Assert.That(siblingElement.XmlText, Is.Not.Null);
    }

    [Test]
    public void GetsPreviousSiblingAtEnding()
    {
        // Arrange
        var (doc, xmlElement) = ArrangeDoc();

        // Act
        var transaction = doc.ReadTransaction();
        var targetXmlElement = xmlElement.Get(transaction, index: 4).XmlElement;
        var siblingElement = targetXmlElement.PreviousSibling(transaction);
        transaction.Commit();

        // Assert
        Assert.That(siblingElement.XmlElement, Is.Null);
        Assert.That(siblingElement.XmlText, Is.Not.Null);
    }

    private (Doc, XmlElement) ArrangeDoc()
    {
        var doc = new Doc();
        var xmlElement = doc.XmlElement("xml-element");

        var transaction = doc.WriteTransaction();
        xmlElement.InsertElement(transaction, index: 0, "width");
        xmlElement.InsertText(transaction, index: 1);
        xmlElement.InsertElement(transaction, index: 2, "color");
        xmlElement.InsertText(transaction, index: 3);
        xmlElement.InsertElement(transaction, index: 4, "border");
        transaction.Commit();

        return (doc, xmlElement);
    }
}
