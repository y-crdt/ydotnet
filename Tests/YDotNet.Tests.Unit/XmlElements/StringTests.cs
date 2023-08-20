using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;

namespace YDotNet.Tests.Unit.XmlElements;

public class StringTests
{
    [Test]
    public void RootLevelNodeHasCorrectStringWithoutChildren()
    {
        // Arrange
        var doc = new Doc();
        var xmlElement = doc.XmlElement("xml-element");

        // Act
        var transaction = doc.ReadTransaction();
        var text = xmlElement.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("<xml-element></xml-element>"));
    }

    [Test]
    public void RootLevelNodeHasCorrectStringWithChildren()
    {
        // TODO [LSViana] Implement checks for content and attribute when XmlText.Insert() is available.

        // Arrange
        var doc = new Doc();
        var xmlElement = doc.XmlElement("xml-element");

        // Act
        var transaction = doc.WriteTransaction();
        xmlElement.InsertElement(transaction, index: 0, "color");
        var text = xmlElement.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("<xml-element><color></color></xml-element>"));
    }

    [Test]
    public void NodeNestedOnMapHasCorrectString()
    {
        // TODO [LSViana] Implement checks for content and attribute when XmlText.Insert() is available.

        // Arrange
        var doc = new Doc();
        var map = doc.Map("map");

        var transaction = doc.WriteTransaction();
        map.Insert(transaction, "xml-element", Input.XmlElement("color"));
        transaction.Commit();

        // Act
        transaction = doc.ReadTransaction();
        var text = map.Get(transaction, "xml-element").XmlElement.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("<color></color>"));
    }

    [Test]
    public void NodeNestedOnArrayHasCorrectString()
    {
        // TODO [LSViana] Implement checks for content and attribute when XmlText.Insert() is available.

        // Arrange
        var doc = new Doc();
        var array = doc.Array("array");

        var transaction = doc.WriteTransaction();
        array.InsertRange(transaction, index: 0, new[] { Input.XmlElement("color") });
        transaction.Commit();

        // Act
        transaction = doc.ReadTransaction();
        var text = array.Get(transaction, index: 0).XmlElement.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("<color></color>"));
    }
}
