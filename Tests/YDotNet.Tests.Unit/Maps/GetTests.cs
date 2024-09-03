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
        var (map, transaction) = ArrangeDoc(
            ("name", Input.String("Lucas")),
            ("earth-🌍", Input.String("globe-🌐"))
        );

        // Act
        var value1 = map.Get(transaction, "name").String;
        var value2 = map.Get(transaction, "earth-🌍").String;

        // Assert
        Assert.That(value1, Is.EqualTo("Lucas"));
        Assert.That(value2, Is.EqualTo("globe-🌐"));
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
        var value2 = map.Get(transaction, "value2");

        // Assert
        Assert.That(value1, Is.EqualTo(new byte[] { 2, 4, 6, 9 }));
        Assert.That(value2.Tag, Is.EqualTo(OutputTag.Boolean));
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
        var value1 = map.Get(transaction, "value1").JsonArray;
        var value2 = map.Get(transaction, "value2");

        // Assert
        //Assert.That(value1, Is.EqualTo(new byte[] { 2, 4, 6, 9 }));
        Assert.That(value1.Count, Is.EqualTo(expected: 2));
        Assert.That(value1[index: 0].Long, Is.EqualTo(expected: 2469));
        Assert.That(value1[index: 1].Long, Is.EqualTo(expected: -420L));
        Assert.That(value2.Tag, Is.EqualTo(OutputTag.Boolean));
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
        var value1 = map.Get(transaction, "value1").JsonObject;
        var value2 = map.Get(transaction, "value2");

        // Assert
        Assert.That(value1.Keys.Count, Is.EqualTo(expected: 2));
        Assert.That(value1["star-⭐"].Long, Is.EqualTo(expected: 2469));
        Assert.That(value1["moon-🌕"].Long, Is.EqualTo(expected: -420L));
        Assert.That(value2.Tag, Is.EqualTo(OutputTag.Boolean));
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
        Assert.That(value1.Tag, Is.EqualTo(OutputTag.Null));
        Assert.That(value2.Tag, Is.Not.EqualTo(OutputTag.Null));
        Assert.That(value3.Tag, Is.Not.EqualTo(OutputTag.Null));
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
        Assert.That(value1.Tag, Is.EqualTo(OutputTag.Undefined));
        Assert.That(value2.Tag, Is.Not.EqualTo(OutputTag.Undefined));
        Assert.That(value3.Tag, Is.Not.EqualTo(OutputTag.Undefined));
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
        var value2 = map.Get(transaction, "value2");

        // Assert
        Assert.That(value1, Is.Not.Null);
        Assert.That(value1.String(transaction), Is.EqualTo("Lucas"));
        Assert.That(value2.Tag, Is.EqualTo(OutputTag.Boolean));
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
        var value1Length = value1.Length(transaction);
        var value2 = map.Get(transaction, "value2");

        // Assert
        Assert.That(value1, Is.Not.Null);
        Assert.That(value1Length, Is.EqualTo(expected: 2));
        Assert.That(value2.Tag, Is.EqualTo(OutputTag.Null));
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
        var value2 = map.Get(transaction, "value2");

        // Assert
        Assert.That(value1, Is.Not.Null);
        Assert.That(value1.Length(transaction), Is.EqualTo(expected: 2));
        Assert.That(value1.Get(transaction, "value1-1").Long, Is.EqualTo(expected: 2469L));
        Assert.That(value1.Get(transaction, "value1-2").Long, Is.EqualTo(expected: -420L));
        Assert.That(value2.Tag, Is.EqualTo(OutputTag.Boolean));
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
        var value2 = map.Get(transaction, "value2");

        // Assert
        Assert.That(value1, Is.Not.Null);
        Assert.That(value1.Tag(transaction), Is.EqualTo("person"));
        Assert.That(value2.Tag, Is.EqualTo(OutputTag.Null));
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
        var value2 = map.Get(transaction, "value2");

        // Assert
        Assert.That(value1, Is.Not.Null);
        Assert.That(value1.Length(transaction), Is.EqualTo(expected: 5));
        Assert.That(value2.Tag, Is.EqualTo(OutputTag.Null));
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

        foreach (var (kez, value) in values)
        {
            map.Insert(transaction, kez, value);
        }

        return (map, transaction);
    }
}
