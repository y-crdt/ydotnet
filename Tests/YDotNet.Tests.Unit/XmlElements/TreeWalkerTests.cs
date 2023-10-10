using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.XmlElements;

public class TreeWalkerTests
{
    [Test]
    public void WalksOnEmptyTree()
    {
        // Arrange
        var doc = new Doc();
        var xmlElement = doc.XmlElement("xml-element");

        // Act
        var transaction = doc.ReadTransaction();
        var xmlTreeWalker = xmlElement.TreeWalker(transaction);
        transaction.Commit();

        // Assert
        Assert.That(xmlTreeWalker, Is.Not.Null);
        Assert.That(xmlTreeWalker.ToArray().Length, Is.EqualTo(expected: 0));
    }

    [Test]
    public void WalksOnTreeWithSingleLevelOfDepth()
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
        var xmlTreeWalker = xmlElement.TreeWalker(transaction);
        transaction.Commit();

        // Assert
        var xmlNodes = xmlTreeWalker.ToArray();

        Assert.That(xmlTreeWalker, Is.Not.Null);
        Assert.That(xmlNodes.Length, Is.EqualTo(expected: 3));
        Assert.That(xmlNodes.ElementAt(index: 0).ResolveXmlText(), Is.Not.Null);
        Assert.That(xmlNodes.ElementAt(index: 1).ResolveXmlElement(), Is.Not.Null);
        Assert.That(xmlNodes.ElementAt(index: 1).ResolveXmlElement().Tag, Is.EqualTo("color"));
        Assert.That(xmlNodes.ElementAt(index: 2).ResolveXmlText(), Is.Not.Null);
    }

    [Test]
    public void WalksOnTreeWithMultipleLevelsOfDepth()
    {
        // Arrange
        var doc = new Doc();
        var xmlElement = doc.XmlElement("xml-element");

        var transaction = doc.WriteTransaction();
        xmlElement.InsertText(transaction, index: 0);
        xmlElement.InsertElement(transaction, index: 1, "color");
        var childXmlElement = xmlElement.Get(transaction, index: 1).ResolveXmlElement();
        childXmlElement.InsertElement(transaction, index: 0, "alpha");
        childXmlElement.InsertElement(transaction, index: 1, "hex");
        childXmlElement.InsertText(transaction, index: 2);
        transaction.Commit();

        // Act
        transaction = doc.ReadTransaction();
        var xmlTreeWalker = xmlElement.TreeWalker(transaction);
        transaction.Commit();

        // Assert
        var xmlNodes = xmlTreeWalker.ToArray();

        Assert.That(xmlTreeWalker, Is.Not.Null);
        Assert.That(xmlNodes.Length, Is.EqualTo(expected: 5));
        Assert.That(xmlNodes.ElementAt(index: 0).ResolveXmlText(), Is.Not.Null);
        Assert.That(xmlNodes.ElementAt(index: 1).ResolveXmlElement(), Is.Not.Null);
        Assert.That(xmlNodes.ElementAt(index: 1).ResolveXmlElement().Tag, Is.EqualTo("color"));
        Assert.That(xmlNodes.ElementAt(index: 2).ResolveXmlElement(), Is.Not.Null);
        Assert.That(xmlNodes.ElementAt(index: 2).ResolveXmlElement().Tag, Is.EqualTo("alpha"));
        Assert.That(xmlNodes.ElementAt(index: 3).ResolveXmlElement(), Is.Not.Null);
        Assert.That(xmlNodes.ElementAt(index: 3).ResolveXmlElement().Tag, Is.EqualTo("hex"));
        Assert.That(xmlNodes.ElementAt(index: 4).ResolveXmlText(), Is.Not.Null);
    }
}
