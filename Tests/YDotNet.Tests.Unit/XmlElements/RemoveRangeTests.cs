using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Types.XmlElements;

namespace YDotNet.Tests.Unit.XmlElements;

public class RemoveRangeTests
{
    [Test]
    public void RemoveEmptyRangeOnEmptyElement()
    {
        // Arrange
        var doc = new Doc();
        var xmlElement = doc.XmlElement("xml-element");

        // Act
        var transaction = doc.WriteTransaction();
        xmlElement.RemoveRange(transaction, index: 0, length: 0);
        var childLength = xmlElement.ChildLength(transaction);
        transaction.Commit();

        // Assert
        Assert.That(childLength, Is.EqualTo(expected: 0));
    }

    [Test]
    public void RemoveEmptyRangeOnFilledElement()
    {
        // Arrange
        var (doc, xmlElement) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        xmlElement.RemoveRange(transaction, index: 2, length: 0);
        var childLength = xmlElement.ChildLength(transaction);
        transaction.Commit();

        // Assert
        Assert.That(childLength, Is.EqualTo(expected: 5));
    }

    [Test]
    public void RemoveSingleItemRange()
    {
        // Arrange
        var (doc, xmlElement) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        xmlElement.RemoveRange(transaction, index: 3, length: 1);
        var childLength = xmlElement.ChildLength(transaction);
        transaction.Commit();

        // Assert
        Assert.That(childLength, Is.EqualTo(expected: 4));
    }

    [Test]
    public void RemoveMultipleItemsRange()
    {
        // Arrange
        var (doc, xmlElement) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        xmlElement.RemoveRange(transaction, index: 2, length: 3);
        var childLength = xmlElement.ChildLength(transaction);
        transaction.Commit();

        // Assert
        Assert.That(childLength, Is.EqualTo(expected: 2));
    }

    [Test]
    public void RemoveAllItemsAtOnce()
    {
        // Arrange
        var (doc, xmlElement) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        xmlElement.RemoveRange(transaction, index: 0, length: 5);
        var childLength = xmlElement.ChildLength(transaction);
        transaction.Commit();

        // Assert
        Assert.That(childLength, Is.EqualTo(expected: 0));
    }

    private (Doc, XmlElement) ArrangeDoc()
    {
        var doc = new Doc();
        var xmlElement = doc.XmlElement("xml-element");

        var transaction = doc.WriteTransaction();
        xmlElement.InsertText(transaction, index: 0);
        xmlElement.InsertText(transaction, index: 1);
        xmlElement.InsertElement(transaction, index: 2, "color");
        xmlElement.InsertText(transaction, index: 3);
        xmlElement.InsertElement(transaction, index: 4, "border");
        transaction.Commit();

        return (doc, xmlElement);
    }
}
