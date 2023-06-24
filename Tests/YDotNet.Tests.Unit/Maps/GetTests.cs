using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;
using YDotNet.Document.Transactions;
using YDotNet.Document.Types;

namespace YDotNet.Tests.Unit.Maps;

public class GetTests
{
    [Test]
    public void GetBoolean()
    {
        // Arrange
        var (map, transaction) = ArrangeDoc(
            ("value1", Input.Boolean(value: true)),
            ("value2", Input.Boolean(value: false))
        );

        // Act
        var value1 = map.Get(transaction, "value1").Boolean;
        var value2 = map.Get(transaction, "value2").Boolean;

        // Assert
        Assert.That(value1, Is.True);
        Assert.That(value2, Is.False);
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
        var (map, transaction) = ArrangeDoc(("name", Input.String("Lucas")));

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
        var (map, transaction) = ArrangeDoc(("sub-doc", Input.Doc(subDoc)));

        // Act
        var subDocFromMap = map.Get(transaction, "sub-doc").Doc;

        // Assert
        Assert.That(subDoc.Id, Is.EqualTo(subDocFromMap.Id));
    }

    [Test]
    [Ignore("To be implemented.")]
    public void GetWrongTypeOnExistingKeyReturnsNull()
    {
    }

    [Test]
    [Ignore("To be implemented.")]
    public void GetNewKeyReturnsNull()
    {
    }

    private (Map?, Transaction?) ArrangeDoc(params (string Key, Input Value)[] values)
    {
        var doc = new Doc();
        var map = doc.Map("map");
        var transaction = doc.WriteTransaction();

        foreach (var value in values)
        {
            map.Insert(transaction, value.Key, value.Value);
        }

        return (map, transaction);
    }
}
