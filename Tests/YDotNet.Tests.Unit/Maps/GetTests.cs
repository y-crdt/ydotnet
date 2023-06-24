using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;
using YDotNet.Document.Transactions;
using YDotNet.Document.Types;

namespace YDotNet.Tests.Unit.Maps;

public class GetTests
{
    [Test]
    [Ignore("To be implemented.")]
    public void GetBoolean()
    {
    }

    [Test]
    [Ignore("To be implemented.")]
    public void GetDouble()
    {
    }

    [Test]
    [Ignore("To be implemented.")]
    public void GetLong()
    {
    }

    [Test]
    public void GetString()
    {
        // Arrange
        var (map, transaction) = ArrangeDoc("name", new Input("Lucas"));

        // Act
        var value = map.Get(transaction, "name").String;

        // Assert
        Assert.That(value, Is.EqualTo("Lucas"));
    }

    [Test]
    [Ignore("To be implemented.")]
    public void GetBytes()
    {
    }

    [Test]
    [Ignore("To be implemented.")]
    public void GetCollection()
    {
    }

    [Test]
    [Ignore("To be implemented.")]
    public void GetObject()
    {
    }

    [Test]
    [Ignore("To be implemented.")]
    public void GetNull()
    {
    }

    [Test]
    [Ignore("To be implemented.")]
    public void GetUndefined()
    {
    }

    [Test]
    [Ignore("To be implemented.")]
    public void GetText()
    {
    }

    [Test]
    [Ignore("To be implemented.")]
    public void GetArray()
    {
    }

    [Test]
    [Ignore("To be implemented.")]
    public void GetMap()
    {
    }

    [Test]
    [Ignore("To be implemented.")]
    public void GetXmlElement()
    {
    }

    [Test]
    [Ignore("To be implemented.")]
    public void GetXmlText()
    {
    }

    [Test]
    public void GetDoc()
    {
        // Arrange
        var subDoc = new Doc();
        var (map, transaction) = ArrangeDoc("sub-doc", new Input(subDoc));

        // Act
        var subDocFromMap = map.Get(transaction, "sub-doc").Doc;

        // Assert
        Assert.That(subDoc.Id, Is.EqualTo(subDocFromMap.Id));
    }

    private (Map?, Transaction?) ArrangeDoc(string key, Input input)
    {
        var doc = new Doc();
        var map = doc.Map("map");
        var transaction = doc.WriteTransaction();

        map.Insert(transaction, key, input);

        return (map, transaction);
    }
}
