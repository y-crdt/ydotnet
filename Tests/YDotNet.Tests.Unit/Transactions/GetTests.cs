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
        var xmlFragment = transaction.GetXmlFragment("xml-element");
        transaction.Commit();

        // Assert
        Assert.That(array, Is.Null);
        Assert.That(map, Is.Null);
        Assert.That(text, Is.Null);
        Assert.That(xmlFragment, Is.Null);
    }

    [Test]
    public void GetOnDefinedKeyWithCorrectTypeReturnsValue()
    {
        // Arrange
        var doc = new Doc();
        doc.Array("array");
        doc.Map("map");
        doc.Text("text");
        doc.XmlFragment("xml-fragment");

        // Act
        var transaction = doc.ReadTransaction();
        var array = transaction.GetArray("array");
        var map = transaction.GetMap("map");
        var text = transaction.GetText("text");
        var xmlFragment = transaction.GetXmlFragment("xml-fragment");
        transaction.Commit();

        // Assert
        Assert.That(array, Is.Not.Null);
        Assert.That(map, Is.Not.Null);
        Assert.That(text, Is.Not.Null);
        Assert.That(xmlFragment, Is.Not.Null);
    }

    [Test]
    public void GetOnDefinedKeyWithWrongTypeThrowsException()
    {
        // Arrange
        var doc = new Doc();
        doc.Array("array");
        doc.Map("map");
        doc.Text("text");
        doc.XmlFragment("xml-fragment");

        // Act
        var transaction = doc.ReadTransaction();
        Assert.Throws<YDotNetException>(() => transaction.GetArray("map"));
        Assert.Throws<YDotNetException>(() => transaction.GetMap("text"));
        Assert.Throws<YDotNetException>(() => transaction.GetText("xml-fragment"));
        Assert.Throws<YDotNetException>(() => transaction.GetXmlFragment("array"));
        transaction.Commit();
    }
}
