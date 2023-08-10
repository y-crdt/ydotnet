using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;

namespace YDotNet.Tests.Unit.XmlElements;

public class StringTests
{
    [Test]
    [Ignore("Check the Tag implementation for root-level nodes.")]
    public void RootLevelNodeIsEmptyByDefault()
    {
        // Arrange
        var doc = new Doc();
        var xmlElement = doc.XmlElement("xml-element");

        // Act
        var text = xmlElement.String;

        // Assert
        Assert.That(text, Is.Null);
    }

    [Test]
    [Ignore("Waiting for the implementation of XmlElement.InsertText().")]
    public void RootLevelNodeHasCorrectStringWithoutChildren()
    {
        // TODO [LSViana] Implement this test when XmlElement.InsertText() is available.
    }

    [Test]
    [Ignore("Waiting for the implementation of XmlElement.InsertText().")]
    public void RootLevelNodeHasCorrectStringWithChildren()
    {
        // TODO [LSViana] Implement this test when XmlElement.InsertText() is available.
    }

    [Test]
    public void NodeNestedOnMapHasCorrectString()
    {
        // TODO [LSViana] Implement checks for content and attribute when XmlElement.InsertText() is available.

        // Arrange
        var doc = new Doc();
        var map = doc.Map("map");

        var transaction = doc.WriteTransaction();
        map.Insert(transaction, "xml-element", Input.XmlElement("color"));
        transaction.Commit();

        // Act
        transaction = doc.ReadTransaction();
        var text = map.Get(transaction, "xml-element").XmlElement.String;
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("<color></color>"));
    }

    [Test]
    public void NodeNestedOnArrayHasCorrectString()
    {
        // TODO [LSViana] Implement checks for content and attribute when XmlElement.InsertText() is available.

        // Arrange
        var doc = new Doc();
        var array = doc.Array("array");

        var transaction = doc.WriteTransaction();
        array.InsertRange(transaction, index: 0, new[] { Input.XmlElement("color") });
        transaction.Commit();

        // Act
        transaction = doc.ReadTransaction();
        var text = array.Get(transaction, index: 0).XmlElement.String;
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("<color></color>"));
    }

    [Test]
    public void ToStringIsOverriddenToInvokeString()
    {
        // TODO [LSViana] Implement checks for content and attribute when XmlElement.InsertText() is available.

        // Arrange
        var doc = new Doc();
        var map = doc.Map("map");

        var transaction = doc.WriteTransaction();
        map.Insert(transaction, "xml-element", Input.XmlElement("color"));
        transaction.Commit();

        // Act
        transaction = doc.ReadTransaction();
        var text = map.Get(transaction, "xml-element").XmlElement.ToString();
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("<color></color>"));
    }
}
