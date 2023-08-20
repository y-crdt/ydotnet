using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;

namespace YDotNet.Tests.Unit.XmlElements;

public class ParentTests
{
    [Test]
    public void ParentOfRootNodeReturnsNull()
    {
        // Arrange
        var doc = new Doc();
        var xmlElement = doc.XmlElement("xml-element");

        // Act
        var transaction = doc.ReadTransaction();
        var parentXmlElement = xmlElement.Parent(transaction);
        transaction.Commit();

        // Assert
        Assert.That(parentXmlElement, Is.Null);
    }

    [Test]
    public void ParentOfNodeInsideArrayReturnsNull()
    {
        // Arrange
        var doc = new Doc();
        var array = doc.Array("array");

        var transaction = doc.WriteTransaction();
        array.InsertRange(transaction, index: 0, new[] { Input.XmlElement("attributes") });
        transaction.Commit();

        // Act
        transaction = doc.ReadTransaction();
        var xmlElement = array.Get(transaction, index: 0).XmlElement;
        var parentXmlElement = xmlElement.Parent(transaction);
        transaction.Commit();

        // Assert
        Assert.That(parentXmlElement, Is.Null);
    }

    [Test]
    public void ParentOfNodeInsideMapReturnsNull()
    {
        // Arrange
        var doc = new Doc();
        var map = doc.Map("array");

        var transaction = doc.WriteTransaction();
        map.Insert(transaction, "attributes", Input.XmlElement("attributes"));
        transaction.Commit();

        // Act
        transaction = doc.ReadTransaction();
        var xmlElement = map.Get(transaction, "attributes").XmlElement;
        var parentXmlElement = xmlElement.Parent(transaction);
        transaction.Commit();

        // Assert
        Assert.That(parentXmlElement, Is.Null);
    }

    [Test]
    public void ParentOfNestedNodeAtFirstLevelReturnsCorrectParent()
    {
        // Arrange
        var doc = new Doc();
        var xmlElement = doc.XmlElement("xml-element");

        var transaction = doc.WriteTransaction();
        xmlElement.InsertElement(transaction, index: 0, "width");
        xmlElement.InsertElement(transaction, index: 1, "color");
        transaction.Commit();

        // Act
        transaction = doc.ReadTransaction();
        var childXmlElement = xmlElement.Get(transaction, index: 0).XmlElement;

        var parentXmlElement = childXmlElement.Parent(transaction);
        var childLength = parentXmlElement.ChildLength(transaction);
        transaction.Commit();

        // Assert
        Assert.That(parentXmlElement, Is.Not.Null);
        Assert.That(childLength, Is.EqualTo(expected: 2));
    }

    [Test]
    public void ParentOfNestedNodeAtSecondLevelReturnsCorrectParent()
    {
        // Arrange
        var doc = new Doc();
        var xmlElement = doc.XmlElement("xml-element");

        var transaction = doc.WriteTransaction();
        var childXmlElement = xmlElement.InsertElement(transaction, index: 0, "dimensions");
        childXmlElement.InsertElement(transaction, index: 0, "width");
        childXmlElement.InsertElement(transaction, index: 0, "height");
        var grandChildXmlElement = childXmlElement.InsertElement(transaction, index: 0, "depth");
        transaction.Commit();

        // Act
        transaction = doc.ReadTransaction();
        var parentXmlElement = grandChildXmlElement.Parent(transaction);
        var childLength = parentXmlElement.ChildLength(transaction);
        transaction.Commit();

        // Assert
        Assert.That(parentXmlElement, Is.Not.Null);
        Assert.That(childLength, Is.EqualTo(expected: 3));
    }
}
