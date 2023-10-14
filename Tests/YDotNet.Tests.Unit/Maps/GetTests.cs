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
        var value1 = map.Get(transaction, "star-⭐");
        var value2 = map.Get(transaction, "moon-🌕");

        // Assert
        Assert.That(value1.Boolean, Is.True);
        Assert.That(value2.Boolean, Is.False);
        Assert.That(value1.Type, Is.EqualTo(OutputType.Boolean));
        Assert.That(value2.Type, Is.EqualTo(OutputType.Boolean));
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
        var value1 = map.Get(transaction, "value1");
        var value2 = map.Get(transaction, "value2");

        // Assert
        Assert.That(value1.Double, Is.EqualTo(expected: 24.69));
        Assert.That(value2.Double, Is.EqualTo(expected: -4.20));
        Assert.That(value1.Type, Is.EqualTo(OutputType.Double));
        Assert.That(value2.Type, Is.EqualTo(OutputType.Double));
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
        var value1 = map.Get(transaction, "value1");
        var value2 = map.Get(transaction, "value2");

        // Assert
        Assert.That(value1.Long, Is.EqualTo(expected: 2469L));
        Assert.That(value2.Long, Is.EqualTo(expected: -420L));
        Assert.That(value1.Type, Is.EqualTo(OutputType.Long));
        Assert.That(value2.Type, Is.EqualTo(OutputType.Long));
    }

    [Test]
    public void GetString()
    {
        // Arrange
        var (map, transaction) = ArrangeDoc(
            ("name", Input.String("Lucas")),
            ("earth-🌍", Input.String("globe-🌐"))
        );

        // Act
        var value1 = map.Get(transaction, "name");
        var value2 = map.Get(transaction, "earth-🌍");

        // Assert
        Assert.That(value1.String, Is.EqualTo("Lucas"));
        Assert.That(value2.String, Is.EqualTo("globe-🌐"));
        Assert.That(value1.Type, Is.EqualTo(OutputType.String));
        Assert.That(value2.Type, Is.EqualTo(OutputType.String));
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
        var value1 = map.Get(transaction, "value1");
        var value2 = map.Get(transaction, "value2");

        // Assert
        Assert.That(value1.Bytes, Is.EqualTo(new byte[] { 2, 4, 6, 9 }));
        Assert.That(value2.Bytes, Is.Null);
        Assert.That(value1.Type, Is.EqualTo(OutputType.Bytes));
        Assert.That(value2.Type, Is.EqualTo(OutputType.Boolean));
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
        var value1 = map.Get(transaction, "value1");
        var value2 = map.Get(transaction, "value2");
        var value1Collection = value1.Collection;

        // Assert
        Assert.That(value1Collection, Is.Not.Null);
        Assert.That(value1Collection.Length, Is.EqualTo(expected: 2));
        Assert.That(value1Collection[0].Long, Is.EqualTo(expected: 2469));
        Assert.That(value1Collection[1].Long, Is.EqualTo(expected: -420L));
        Assert.That(value2.Collection, Is.Null);
        Assert.That(value1.Type, Is.EqualTo(OutputType.Collection));
        Assert.That(value2.Type, Is.EqualTo(OutputType.Boolean));
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
        var value1 = map.Get(transaction, "value1");
        var value2 = map.Get(transaction, "value2");
        var value1Object = value1.Object;

        // Assert
        Assert.That(value1Object, Is.Not.Null);
        Assert.That(value1Object.Count, Is.EqualTo(expected: 2));
        Assert.That(value1Object["star-⭐"].Long, Is.EqualTo(expected: 2469));
        Assert.That(value1Object["moon-🌕"].Long, Is.EqualTo(expected: -420L));
        Assert.That(value2.Object, Is.Null);
        Assert.That(value1.Type, Is.EqualTo(OutputType.Object));
        Assert.That(value2.Type, Is.EqualTo(OutputType.Boolean));
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
        var value1 = map.Get(transaction, "value1");
        var value2 = map.Get(transaction, "value2");
        var value3 = map.Get(transaction, "value3");

        // Assert
        Assert.That(value1.Null, Is.True);
        Assert.That(value2.Null, Is.False);
        Assert.That(value3.Null, Is.False);
        Assert.That(value1.Type, Is.EqualTo(OutputType.Null));
        Assert.That(value2.Type, Is.EqualTo(OutputType.Undefined));
        Assert.That(value3.Type, Is.EqualTo(OutputType.Boolean));
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
        var value1 = map.Get(transaction, "value1");
        var value2 = map.Get(transaction, "value2");
        var value3 = map.Get(transaction, "value3");

        // Assert
        Assert.That(value1.Undefined, Is.True);
        Assert.That(value2.Undefined, Is.False);
        Assert.That(value3.Undefined, Is.False);
        Assert.That(value1.Type, Is.EqualTo(OutputType.Undefined));
        Assert.That(value2.Type, Is.EqualTo(OutputType.Null));
        Assert.That(value3.Type, Is.EqualTo(OutputType.Boolean));
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
        var value1 = map.Get(transaction, "value1");
        var value2 = map.Get(transaction, "value2");

        // Assert
        Assert.That(value1.Text.String(transaction), Is.EqualTo("Lucas"));
        Assert.That(value2.Text, Is.Null);
        Assert.That(value1.Type, Is.EqualTo(OutputType.Text));
        Assert.That(value2.Type, Is.EqualTo(OutputType.Boolean));
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
        var value1 = map.Get(transaction, "value1");
        var value2 = map.Get(transaction, "value2");
        var value1Array = value1.Array;

        // Assert
        Assert.That(value1Array, Is.Not.Null);
        Assert.That(value1Array.Length, Is.EqualTo(expected: 2));
        Assert.That(value2.Array, Is.Null);
        Assert.That(value1.Type, Is.EqualTo(OutputType.Array));
        Assert.That(value2.Type, Is.EqualTo(OutputType.Null));
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
        var value1 = map.Get(transaction, "value1");
        var value2 = map.Get(transaction, "value2");
        var value1Map = value1.Map;

        // Assert
        Assert.That(value1Map, Is.Not.Null);
        Assert.That(value1Map.Length(transaction), Is.EqualTo(expected: 2));
        Assert.That(value1Map.Get(transaction, "value1-1").Long, Is.EqualTo(expected: 2469L));
        Assert.That(value1Map.Get(transaction, "value1-2").Long, Is.EqualTo(expected: -420L));
        Assert.That(value2.Map, Is.Null);
        Assert.That(value1.Type, Is.EqualTo(OutputType.Map));
        Assert.That(value2.Type, Is.EqualTo(OutputType.Boolean));
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
        var value1 = map.Get(transaction, "value1");
        var value2 = map.Get(transaction, "value2");
        var value1XmlElement = value1.XmlElement;

        // Assert
        Assert.That(value1XmlElement, Is.Not.Null);
        Assert.That(value1XmlElement.Tag, Is.EqualTo("person"));
        Assert.That(value2.XmlElement, Is.Null);
        Assert.That(value1.Type, Is.EqualTo(OutputType.XmlElement));
        Assert.That(value2.Type, Is.EqualTo(OutputType.Null));
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
        var value1 = map.Get(transaction, "value1");
        var value2 = map.Get(transaction, "value2");
        var value1XmlText = value1.XmlText;

        // Assert
        Assert.That(value1XmlText, Is.Not.Null);
        Assert.That(value1XmlText.Length(transaction), Is.EqualTo(expected: 5));
        Assert.That(value2.XmlText, Is.Null);
        Assert.That(value1.Type, Is.EqualTo(OutputType.XmlText));
        Assert.That(value2.Type, Is.EqualTo(OutputType.Null));
    }

    [Test]
    public void GetDoc()
    {
        // Arrange
        var subDoc = new Doc();
        var (map, transaction) = ArrangeDoc(("sub-doc", Input.Doc(subDoc)));

        // Act
        var value1 = map.Get(transaction, "sub-doc");

        // Assert
        Assert.That(subDoc.Id, Is.EqualTo(value1.Doc.Id));
        Assert.That(value1.Type, Is.EqualTo(OutputType.Doc));
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
        var value1 = map.Get(transaction, "value1");
        var value2 = map.Get(transaction, "value2");

        // Assert
        Assert.That(value1.Double, Is.Null);
        Assert.That(value2.Long, Is.Null);
        Assert.That(value1.Type, Is.EqualTo(OutputType.Long));
        Assert.That(value2.Type, Is.EqualTo(OutputType.Double));
    }

    [Test]
    public void GetNewKeyReturnsNull()
    {
        // Arrange
        var doc = new Doc();
        var map = doc.Map("map");

        // Act
        var transaction = doc.ReadTransaction();
        var value = map.Get(transaction, "new-key");
        transaction.Commit();

        // Assert
        Assert.That(value, Is.Null);
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
