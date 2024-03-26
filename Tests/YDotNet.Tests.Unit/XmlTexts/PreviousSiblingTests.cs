using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Types.XmlElements;

namespace YDotNet.Tests.Unit.XmlTexts;

public class PreviousSiblingTests
{
    [Test]
    public void GetsPreviousSiblingAtBeginning()
    {
        // Arrange
        var (doc, xmlElement) = ArrangeDoc();

        // Act
        var transaction = doc.ReadTransaction();
        var target = xmlElement.Get(transaction, index: 0).XmlText;
        var sibling = target.PreviousSibling(transaction);
        transaction.Commit();

        // Assert
        Assert.That(sibling, Is.Null);
    }

    [Test]
    public void GetsPreviousSiblingAtMiddle()
    {
        // Arrange
        var (doc, xmlElement) = ArrangeDoc();

        // Act
        var transaction = doc.ReadTransaction();
        var target = xmlElement.Get(transaction, index: 2).XmlText;
        var sibling = target.PreviousSibling(transaction);
        transaction.Commit();

        // Assert
        Assert.That(sibling.XmlElement, Is.Not.Null);
    }

    [Test]
    public void GetsPreviousSiblingAtEnding()
    {
        // Arrange
        var (doc, xmlElement) = ArrangeDoc();

        // Act
        var transaction = doc.ReadTransaction();
        var target = xmlElement.Get(transaction, index: 4).XmlText;
        var sibling = target.PreviousSibling(transaction);
        transaction.Commit();

        // Assert
        Assert.That(sibling.XmlElement, Is.Not.Null);
    }

    private (Doc, XmlElement) ArrangeDoc()
    {
        var doc = new Doc();
        var xmlFragment = doc.XmlFragment("xml-fragment");

        var transaction = doc.WriteTransaction();
        var xmlElement = xmlFragment.InsertElement(transaction, index: 0, "xml-element");
        xmlElement.InsertText(transaction, index: 0);
        xmlElement.InsertElement(transaction, index: 1, "width");
        xmlElement.InsertText(transaction, index: 2);
        xmlElement.InsertElement(transaction, index: 3, "color");
        xmlElement.InsertText(transaction, index: 4);
        transaction.Commit();

        return (doc, xmlElement);
    }
}
