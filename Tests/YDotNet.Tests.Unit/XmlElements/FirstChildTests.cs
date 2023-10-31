using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.XmlElements;

public class FirstChildTests
{
    [Test]
    public void FirstChildOfRootEmptyNodeIsNull()
    {
        // Arrange
        var doc = new Doc();
        var xmlElement = doc.XmlElement("xml-element");

        // Act
        var transaction = doc.ReadTransaction();
        var childXmlElement = xmlElement.FirstChild(transaction);
        transaction.Commit();

        // Assert
        Assert.That(childXmlElement, Is.Null);
    }

    [Test]
    public void FirstChildOfRootFilledNodeIsCorrect()
    {
        // Arrange
        var doc = new Doc();
        var xmlElement = doc.XmlElement("xml-element");

        var transaction = doc.WriteTransaction();
        xmlElement.InsertText(transaction, index: 0);
        xmlElement.InsertElement(transaction, index: 1, "color");
        xmlElement.InsertText(transaction, index: 2);
        transaction.Commit();

        // Act
        transaction = doc.ReadTransaction();
        var childXmlElement = xmlElement.FirstChild(transaction);
        transaction.Commit();

        // Assert
        Assert.That(childXmlElement.XmlText, Is.Not.Null);
    }

    [Test]
    public void FirstChildOfNestedEmptyNodeIsNull()
    {
        // Arrange
        var doc = new Doc();
        var xmlElement = doc.XmlElement("xml-element");

        var transaction = doc.WriteTransaction();
        xmlElement.InsertElement(transaction, index: 0, "color");
        var childXmlElement = xmlElement.Get(transaction, index: 0).XmlElement;
        transaction.Commit();

        // Act
        transaction = doc.ReadTransaction();
        var grandChildXmlElement = childXmlElement.FirstChild(transaction);
        transaction.Commit();

        // Assert
        Assert.That(grandChildXmlElement, Is.Null);
    }

    [Test]
    public void FirstChildOfNestedFilledNodeIsCorrect()
    {
        // Arrange
        var doc = new Doc();
        var xmlElement = doc.XmlElement("xml-element");

        var transaction = doc.WriteTransaction();
        xmlElement.InsertElement(transaction, index: 0, "color");
        var childXmlElement = xmlElement.Get(transaction, index: 0).XmlElement;
        childXmlElement.InsertElement(transaction, index: 0, "alpha");
        childXmlElement.InsertElement(transaction, index: 0, "hex");
        transaction.Commit();

        // Act
        transaction = doc.ReadTransaction();
        var grandChildXmlElement = childXmlElement.FirstChild(transaction);
        transaction.Commit();

        // Assert
        Assert.That(grandChildXmlElement.XmlElement, Is.Not.Null);
    }
}
