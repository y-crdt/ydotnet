using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Types.XmlElements;

namespace YDotNet.Tests.Unit.XmlTexts;

public class NextSiblingTests
{
    [Test]
    public void GetsNextSiblingAtBeginning()
    {
        // Arrange
        var (doc, xmlElement) = ArrangeDoc();

        // Act
        var transaction = doc.ReadTransaction();
        var target = xmlElement.Get(transaction, index: 0).XmlText;
        var sibling = target.NextSibling(transaction);
        transaction.Commit();

        // Assert
        Assert.That(sibling.XmlElement, Is.Not.Null);
    }

    [Test]
    public void GetsNextSiblingAtMiddle()
    {
        // Arrange
        var (doc, xmlElement) = ArrangeDoc();

        // Act
        var transaction = doc.ReadTransaction();
        var target = xmlElement.Get(transaction, index: 2).XmlText;
        var sibling = target.NextSibling(transaction);
        transaction.Commit();

        // Assert
        Assert.That(sibling.XmlElement, Is.Not.Null);
    }

    [Test]
    public void GetsNextSiblingAtEnding()
    {
        // Arrange
        var (doc, xmlElement) = ArrangeDoc();

        // Act
        var transaction = doc.ReadTransaction();
        var target = xmlElement.Get(transaction, index: 4).XmlText;
        var sibling = target.NextSibling(transaction);
        transaction.Commit();

        // Assert
        Assert.That(sibling, Is.Null);
    }

    private (Doc, XmlElement) ArrangeDoc()
    {
        var doc = new Doc();
        var xmlElement = doc.XmlElement("xml-element");

        var transaction = doc.WriteTransaction();
        xmlElement.InsertText(transaction, index: 0);
        xmlElement.InsertElement(transaction, index: 1, "width");
        xmlElement.InsertText(transaction, index: 2);
        xmlElement.InsertElement(transaction, index: 3, "color");
        xmlElement.InsertText(transaction, index: 4);
        transaction.Commit();

        return (doc, xmlElement);
    }
}
