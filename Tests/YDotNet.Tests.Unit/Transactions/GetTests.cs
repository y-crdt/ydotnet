using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.Transactions;

public class GetTests
{
    [Test]
    public void GetOnUndefinedKeyReturnsNull()
    {
        // Arrange
        var doc = new Doc();

        // Act
        var transaction = doc.ReadTransaction();
        var array = transaction.GetArray("array");
        var map = transaction.GetMap("map");
        var text = transaction.GetText("text");
        var xmlElement = transaction.GetXmlElement("xml-element");
        var xmlText = transaction.GetXmlText("xml-text");
        transaction.Commit();

        // Assert
        Assert.That(array, Is.Null);
        Assert.That(map, Is.Null);
        Assert.That(text, Is.Null);
        Assert.That(xmlElement, Is.Null);
        Assert.That(xmlText, Is.Null);
    }

    [Test]
    public void GetOnDefinedKeyWithCorrectTypeReturnsValue()
    {
        // Arrange
        var doc = new Doc();
        doc.Array("array");
        doc.Map("map");
        doc.Text("text");
        doc.XmlElement("xml-element");
        doc.XmlText("xml-text");

        // Act
        var transaction = doc.ReadTransaction();
        var array = transaction.GetArray("array");
        var map = transaction.GetMap("map");
        var text = transaction.GetText("text");
        var xmlElement = transaction.GetXmlElement("xml-element");
        var xmlText = transaction.GetXmlText("xml-text");
        transaction.Commit();

        // Assert
        Assert.That(array, Is.Not.Null);
        Assert.That(map, Is.Not.Null);
        Assert.That(text, Is.Not.Null);
        Assert.That(xmlElement, Is.Not.Null);
        Assert.That(xmlText, Is.Not.Null);
    }

    [Test]
    public void GetOnDefinedKeyWithWrongTypeReturnsNull()
    {
        // Arrange
        var doc = new Doc();
        doc.Array("array");
        doc.Map("map");
        doc.Text("text");
        doc.XmlElement("xml-element");
        doc.XmlText("xml-text");

        // Act
        var transaction = doc.ReadTransaction();
        var array = transaction.GetArray("map");
        var map = transaction.GetMap("text");
        var text = transaction.GetText("xml-element");
        var xmlElement = transaction.GetXmlElement("xml-text");
        var xmlText = transaction.GetXmlText("array");
        transaction.Commit();

        // Assert
        Assert.That(array, Is.Null);
        Assert.That(map, Is.Null);
        Assert.That(text, Is.Null);
        Assert.That(xmlElement, Is.Null);
        Assert.That(xmlText, Is.Null);
    }
}
