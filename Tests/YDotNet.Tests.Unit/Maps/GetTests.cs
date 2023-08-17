using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;
using YDotNet.Document.Transactions;
using YDotNet.Document.Types.Maps;

namespace YDotNet.Tests.Unit.Maps;

public class GetTests
{
    [Test]
    public void GetBoolean()
    {
        // Arrange
        var (map, transaction) = ArrangeDoc(
            ("star-⭐", Input.Boolean(value: true)),
            ("moon-🌕", Input.Boolean(value: false))
        );

        // Act
        var value1 = map.Get(transaction, "star-⭐").Boolean;
        var value2 = map.Get(transaction, "moon-🌕").Boolean;

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
                    { "star-⭐", Input.Long(value: 2469L) },
                    { "moon-🌕", Input.Long(value: -420L) }
                })),
            ("value2", Input.Boolean(value: true))
        );

        // Act
        var value1 = map.Get(transaction, "value1").Object;
        var value2 = map.Get(transaction, "value2").Object;

        // Assert
        Assert.That(value1.Keys.Count, Is.EqualTo(expected: 2));
        Assert.That(value1["star-⭐"].Long, Is.EqualTo(expected: 2469));
        Assert.That(value1["moon-🌕"].Long, Is.EqualTo(expected: -420L));
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
    public void GetText()
    {
        // Arrange
        var (map, transaction) = ArrangeDoc(
            ("value1", Input.Text("Lucas")),
            ("value2", Input.Boolean(value: true))
        );

        // Act
        var value1 = map.Get(transaction, "value1").Text;
        var value2 = map.Get(transaction, "value2").Text;

        // Assert
        Assert.That(value1, Is.Not.Null);
        Assert.That(value1.String(transaction), Is.EqualTo("Lucas"));
        Assert.That(value2, Is.Null);
    }

    [Test]
    public void GetArray()
    {
        // Arrange
        var (map, transaction) = ArrangeDoc(
            ("value1", Input.Array(
                new[]
                {
                    Input.Long(value: 2469L),
                    Input.Long(value: -420L)
                })),
            ("value2", Input.Null())
        );

        // Act
        var value1 = map.Get(transaction, "value1").Array;
        var value2 = map.Get(transaction, "value2").Array;

        // Assert
        Assert.That(value1, Is.Not.Null);
        Assert.That(value1.Length, Is.EqualTo(expected: 2));
        Assert.That(value2, Is.Null);
    }

    [Test]
    public void GetMap()
    {
        // Arrange
        var (map, transaction) = ArrangeDoc(
            ("value1", Input.Map(
                new Dictionary<string, Input>
                {
                    { "value1-1", Input.Long(value: 2469L) },
                    { "value1-2", Input.Long(value: -420L) }
                })),
            ("value2", Input.Boolean(value: true))
        );

        // Act
        var value1 = map.Get(transaction, "value1").Map;
        var value2 = map.Get(transaction, "value2").Map;

        // Assert
        Assert.That(value1, Is.Not.Null);
        Assert.That(value1.Length(transaction), Is.EqualTo(expected: 2));
        Assert.That(value1.Get(transaction, "value1-1").Long, Is.EqualTo(expected: 2469L));
        Assert.That(value1.Get(transaction, "value1-2").Long, Is.EqualTo(expected: -420L));
        Assert.That(value2, Is.Null);
    }

    [Test]
    public void GetXmlElement()
    {
        // Arrange
        var (map, transaction) = ArrangeDoc(
            ("value1", Input.XmlElement("person")),
            ("value2", Input.Null())
        );

        // Act
        var value1 = map.Get(transaction, "value1").XmlElement;
        var value2 = map.Get(transaction, "value2").XmlElement;

        // Assert
        Assert.That(value1, Is.Not.Null);
        Assert.That(value1.Tag, Is.EqualTo("person"));
        Assert.That(value2, Is.Null);
    }

    [Test]
    public void GetXmlText()
    {
        // Arrange
        var (map, transaction) = ArrangeDoc(
            ("value1", Input.XmlText("Lucas")),
            ("value2", Input.Null())
        );

        // Act
        var value1 = map.Get(transaction, "value1").XmlText;
        var value2 = map.Get(transaction, "value2").XmlText;

        // Assert
        Assert.That(value1, Is.Not.Null);
        Assert.That(value1.Length(transaction), Is.EqualTo(expected: 5));
        Assert.That(value2, Is.Null);
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
    public void GetWrongTypeOnExistingKeyReturnsNull()
    {
        // Arrange
        var (map, transaction) = ArrangeDoc(
            ("value1", Input.Long(value: 2469L)),
            ("value2", Input.Double(value: 4.20))
        );

        // Act
        var value1 = map.Get(transaction, "value1").Double;
        var value2 = map.Get(transaction, "value2").Long;

        // Assert
        Assert.That(value1, Is.Null);
        Assert.That(value2, Is.Null);
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
