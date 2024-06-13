using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;

namespace YDotNet.Tests.Unit.XmlElements;

public class TagTests
{
    [Test]
    public void RootNodesHaveUndefinedAsTag()
    {
        // Arrange
        var doc = new Doc();
        var xmlFragment = doc.XmlFragment("xml-fragment");

        var transaction = doc.WriteTransaction();
        var xmlElement = xmlFragment.InsertElement(transaction, index: 0, "xml-element");
        transaction.Commit();

        // Act
        transaction = doc.ReadTransaction();
        var tag = xmlElement.Tag(transaction);
        transaction.Commit();

        // Assert
        Assert.That(tag, Is.EqualTo("xml-element"));
    }

    [Test]
    public void NodeNestedOnRootNodeHasCorrectTag()
    {
        // Arrange
        var doc = new Doc();
        var xmlFragment = doc.XmlFragment("xml-fragment");

        // Act
        var transaction = doc.WriteTransaction();
        var childXmlElement = xmlFragment.InsertElement(transaction, index: 0, "color");
        var tag = childXmlElement.Tag(transaction);
        transaction.Commit();

        // Assert
        Assert.That(tag, Is.EqualTo("color"));
    }

    [Test]
    public void NodeNestedOnMapHasCorrectTag()
    {
        // Arrange
        var doc = new Doc();
        var map = doc.Map("map");

        var transaction = doc.WriteTransaction();
        map.Insert(transaction, "xml-element", Input.XmlElement("color"));
        transaction.Commit();

        // Act
        transaction = doc.ReadTransaction();
        var tag = map.Get(transaction, "xml-element").XmlElement.Tag(transaction);
        transaction.Commit();

        // Assert
        Assert.That(tag, Is.EqualTo("color"));
    }

    [Test]
    public void NodeNestedOnArrayHasCorrectTag()
    {
        // Arrange
        var doc = new Doc();
        var array = doc.Array("array");

        var transaction = doc.WriteTransaction();
        array.InsertRange(transaction, index: 0, Input.XmlElement("color"));
        transaction.Commit();

        // Act
        transaction = doc.ReadTransaction();
        var tag = array.Get(transaction, index: 0).XmlElement.Tag(transaction);
        transaction.Commit();

        // Assert
        Assert.That(tag, Is.EqualTo("color"));
    }
}
