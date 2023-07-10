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
    public void GetDouble()
    {
        // Arrange
        var (map, transaction) = ArrangeDoc(
            ("value1", Input.Double(value: 24.69)),
            ("value2", Input.Double(value: -4.20))
        );

        // Act
        var value1 = map.Get(transaction, "value1").Double;
        var value2 = map.Get(transaction, "value2").Double;

        // Assert
        Assert.That(value1, Is.EqualTo(expected: 24.69));
        Assert.That(value2, Is.EqualTo(expected: -4.20));
    }

    [Test]
    public void GetLong()
    {
        // Arrange
        var (map, transaction) = ArrangeDoc(
            ("value1", Input.Long(value: 2469L)),
            ("value2", Input.Long(value: -420L))
        );

        // Act
        var value1 = map.Get(transaction, "value1").Long;
        var value2 = map.Get(transaction, "value2").Long;

        // Assert
        Assert.That(value1, Is.EqualTo(expected: 2469L));
        Assert.That(value2, Is.EqualTo(expected: -420L));
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
    public void GetBytes()
    {
        // Arrange
        var (map, transaction) = ArrangeDoc(
            ("value1", Input.Bytes(new byte[] { 2, 4, 6, 9 })),
            ("value2", Input.Boolean(value: true))
        );

        // Act
        var value1 = map.Get(transaction, "value1").Bytes;
        var value2 = map.Get(transaction, "value2").Bytes;

        // Assert
        Assert.That(value1, Is.EqualTo(new byte[] { 2, 4, 6, 9 }));
        Assert.That(value2, Is.Null);
    }

    [Test]
    public void GetCollection()
    {
        // Arrange
        var (map, transaction) = ArrangeDoc(
            ("value1", Input.Collection(
                new[]
                {
                    Input.Long(value: 2469L),
                    Input.Long(value: -420L)
                })),
            ("value2", Input.Boolean(value: true))
        );

        // Act
        var value1 = map.Get(transaction, "value1").Collection;
        var value2 = map.Get(transaction, "value2").Collection;

        // Assert
        //Assert.That(value1, Is.EqualTo(new byte[] { 2, 4, 6, 9 }));
        Assert.That(value1.Length, Is.EqualTo(expected: 2));
        Assert.That(value1[0].Long, Is.EqualTo(expected: 2469));
        Assert.That(value1[1].Long, Is.EqualTo(expected: -420L));
        Assert.That(value2, Is.Null);
    }

    [Test]
    public void GetObject()
    {
        // Arrange
        var (map, transaction) = ArrangeDoc(
            ("value1", Input.Object(
                new Dictionary<string, Input>
                {
                    { "value1-1", Input.Long(value: 2469L) },
                    { "value1-2", Input.Long(value: -420L) }
                })),
            ("value2", Input.Boolean(value: true))
        );

        // Act
        var value1 = map.Get(transaction, "value1").Object;
        var value2 = map.Get(transaction, "value2").Object;

        // Assert
        //Assert.That(value1, Is.EqualTo(new byte[] { 2, 4, 6, 9 }));
        Assert.That(value1.Keys.Count, Is.EqualTo(expected: 2));
        Assert.That(value1["value1-1"].Long, Is.EqualTo(expected: 2469));
        Assert.That(value1["value1-2"].Long, Is.EqualTo(expected: -420L));
        Assert.That(value2, Is.Null);
    }

    [Test]
    public void GetNull()
    {
        // Arrange
        var (map, transaction) = ArrangeDoc(
            ("value1", Input.Null()),
            ("value2", Input.Undefined()),
            ("value3", Input.Boolean(value: true))
        );

        // Act
        var value1 = map.Get(transaction, "value1").Null;
        var value2 = map.Get(transaction, "value2").Null;
        var value3 = map.Get(transaction, "value3").Null;

        // Assert
        Assert.That(value1, Is.True);
        Assert.That(value2, Is.False);
        Assert.That(value3, Is.False);
    }

    [Test]
    public void GetUndefined()
    {
        // Arrange
        var (map, transaction) = ArrangeDoc(
            ("value1", Input.Undefined()),
            ("value2", Input.Null()),
            ("value3", Input.Boolean(value: true))
        );

        // Act
        var value1 = map.Get(transaction, "value1").Undefined;
        var value2 = map.Get(transaction, "value2").Undefined;
        var value3 = map.Get(transaction, "value3").Undefined;

        // Assert
        Assert.That(value1, Is.True);
        Assert.That(value2, Is.False);
        Assert.That(value3, Is.False);
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
