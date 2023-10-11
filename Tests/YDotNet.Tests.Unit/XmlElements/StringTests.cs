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
        // Arrange
        var doc = new Doc();
        var xmlElement = doc.XmlElement("xml-element");

        // Act
        var transaction = doc.WriteTransaction();
        xmlElement.InsertElement(transaction, index: 0, "color");
        var xmlText = xmlElement.InsertText(transaction, index: 1);
        xmlText.Insert(transaction, index: 0, "red");
        var text = xmlElement.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("<xml-element><color></color>red</xml-element>"));
    }

    [Test]
    public void NodeNestedOnMapHasCorrectString()
    {
        // Arrange
        var doc = new Doc();
        var map = doc.Map("map");

        var transaction = doc.WriteTransaction();
        map.Insert(transaction, "xml-element", Input.XmlElement("color"));
        var xmlElement = map.Get(transaction, "xml-element").XmlElement;
        var xmlText = xmlElement.InsertText(transaction, index: 0);
        xmlText.Insert(transaction, index: 0, "blue");
        transaction.Commit();

        // Act
        transaction = doc.ReadTransaction();
        xmlElement = map.Get(transaction, "xml-element").XmlElement;
        var text = xmlElement.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("<color>blue</color>"));
    }

    [Test]
    public void NodeNestedOnArrayHasCorrectString()
    {
        // Arrange
        var doc = new Doc();
        var array = doc.Array("array");

        var transaction = doc.WriteTransaction();
        array.InsertRange(transaction, index: 0, new[] { Input.XmlElement("color") });
        var xmlElement = array.Get(transaction, index: 0).XmlElement;
        var xmlText = xmlElement.InsertText(transaction, index: 0);
        xmlText.Insert(transaction, index: 0, "purple");
        transaction.Commit();

        // Act
        transaction = doc.ReadTransaction();
        var text = array.Get(transaction, index: 0).XmlElement.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("<color>purple</color>"));
    }
}
