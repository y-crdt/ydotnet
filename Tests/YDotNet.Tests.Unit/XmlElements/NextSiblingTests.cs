using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Types.XmlElements;

namespace YDotNet.Tests.Unit.XmlElements;

public class NextSiblingTests
{
    [Test]
    public void GetsNextSiblingAtBeginning()
    {
        // Arrange
        var (doc, xmlElement) = ArrangeDoc();

        // Act
        var transaction = doc.ReadTransaction();
        var target = xmlElement.Get(transaction, index: 0).XmlElement;
        var sibling = target.NextSibling(transaction);
        transaction.Commit();

        // Assert
        Assert.That(sibling.XmlText, Is.Not.Null);
    }

    [Test]
    public void GetsNextSiblingAtMiddle()
    {
        // Arrange
        var (doc, xmlElement) = ArrangeDoc();

        // Act
        var transaction = doc.ReadTransaction();
        var target = xmlElement.Get(transaction, index: 2).XmlElement;
        var sibling = target.NextSibling(transaction);
        transaction.Commit();

        // Assert
        Assert.That(sibling.XmlText, Is.Not.Null);
    }

    [Test]
    public void GetsNextSiblingAtEnding()
    {
        // Arrange
        var (doc, xmlElement) = ArrangeDoc();

        // Act
        var transaction = doc.ReadTransaction();
        var target = xmlElement.Get(transaction, index: 4).XmlElement;
        var sibling = target.NextSibling(transaction);
        transaction.Commit();

        // Assert
        Assert.That(sibling, Is.Null);
    }

    private (Doc, XmlElement) ArrangeDoc()
    {
        var doc = new Doc();
        var xmlFragment = doc.XmlFragment("xml-fragment");

        var transaction = doc.WriteTransaction();
        var xmlElement = xmlFragment.InsertElement(transaction, 0, "xml-element");
        transaction.Commit();

        // Act
        transaction = doc.WriteTransaction();
        xmlElement.InsertElement(transaction, index: 0, "width");
        xmlElement.InsertText(transaction, index: 1);
        xmlElement.InsertElement(transaction, index: 2, "color");
        xmlElement.InsertText(transaction, index: 3);
        xmlElement.InsertElement(transaction, index: 4, "border");
        transaction.Commit();

        return (doc, xmlElement);
    }
}
