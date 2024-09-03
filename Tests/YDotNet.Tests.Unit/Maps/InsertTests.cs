using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;
using YDotNet.Document.Types.Maps;

namespace YDotNet.Tests.Unit.Maps;

public class InsertTests
{
    [Test]
    public void InsertBoolean()
    {
        // Arrange
        var (doc, map) = ArrangeDoc();
        var value1 = true;
        var value2 = false;

        // Act
        var transaction = doc.WriteTransaction();
        map.Insert(transaction, "value1", Input.Boolean(value1));
        map.Insert(transaction, "value2", Input.Boolean(value2));
        var length = map.Length(transaction);
        transaction.Commit();

        // Assert
        Assert.That(length, Is.EqualTo(expected: 2));
    }

    [Test]
    public void InsertDouble()
    {
        // Arrange
        var (doc, map) = ArrangeDoc();
        var value1 = 24.69;
        var value2 = -4.20;

        // Act
        var transaction = doc.WriteTransaction();
        map.Insert(transaction, "value1", Input.Double(value1));
        map.Insert(transaction, "value2", Input.Double(value2));
        var length = map.Length(transaction);
        transaction.Commit();

        // Assert
        Assert.That(length, Is.EqualTo(expected: 2));
    }

    [Test]
    public void InsertLong()
    {
        // Arrange
        var (doc, map) = ArrangeDoc();
        var value1 = 2469L;
        var value2 = -420L;

        // Act
        var transaction = doc.WriteTransaction();
        map.Insert(transaction, "value1", Input.Long(value1));
        map.Insert(transaction, "value2", Input.Long(value2));
        var length = map.Length(transaction);
        transaction.Commit();

        // Assert
        Assert.That(length, Is.EqualTo(expected: 2));
    }

    [Test]
    public void InsertString()
    {
        // Arrange
        var (doc, map) = ArrangeDoc();
        var name = "Lucas";

        // Act
        var transaction = doc.WriteTransaction();
        map.Insert(transaction, "name", Input.String(name));
        var length = map.Length(transaction);
        transaction.Commit();

        // Assert
        Assert.That(length, Is.EqualTo(expected: 1));
    }

    [Test]
    public void InsertBytes()
    {
        // Arrange
        var (doc, map) = ArrangeDoc();
        var value = new byte[] { 2, 4, 6, 9 };

        // Act
        var transaction = doc.WriteTransaction();
        map.Insert(transaction, "value", Input.Bytes(value));
        var length = map.Length(transaction);
        transaction.Commit();

        // Assert
        Assert.That(length, Is.EqualTo(expected: 1));
    }

    [Test]
    public void InsertCollection()
    {
        // Arrange
        var (doc, map) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        map.Insert(
            transaction, "value", Input.Collection(
                new[]
                {
                    Input.Long(value: 2469L),
                    Input.Long(value: -420L)
                }));
        var length = map.Length(transaction);
        transaction.Commit();

        // Assert
        Assert.That(length, Is.EqualTo(expected: 1));
    }

    [Test]
    public void InsertObject()
    {
        // Arrange
        var (doc, map) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        map.Insert(
            transaction, "value", Input.Object(
                new Dictionary<string, Input>
                {
                    { "value1", Input.Long(value: 2469L) },
                    { "value2", Input.Long(value: -420L) }
                }));
        var length = map.Length(transaction);
        transaction.Commit();

        // Assert
        Assert.That(length, Is.EqualTo(expected: 1));
    }

    [Test]
    public void InsertNull()
    {
        // Arrange
        var (doc, map) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        map.Insert(transaction, "value", Input.Null());
        var length = map.Length(transaction);
        transaction.Commit();

        // Assert
        Assert.That(length, Is.EqualTo(expected: 1));
    }

    [Test]
    public void InsertUndefined()
    {
        // Arrange
        var (doc, map) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        map.Insert(transaction, "value", Input.Undefined());
        var length = map.Length(transaction);
        transaction.Commit();

        // Assert
        Assert.That(length, Is.EqualTo(expected: 1));
    }

    [Test]
    public void InsertText()
    {
        // Arrange
        var (doc, map) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        map.Insert(transaction, "value", Input.Text("Lucas"));
        var length = map.Length(transaction);
        transaction.Commit();

        // Assert
        Assert.That(length, Is.EqualTo(expected: 1));
    }

    [Test]
    public void InsertArray()
    {
        // Arrange
        var (doc, map) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        map.Insert(
            transaction, "value", Input.Array(
                new[]
                {
                    Input.Long(value: 2469L),
                    Input.Long(value: -420L)
                }));
        var length = map.Length(transaction);
        transaction.Commit();

        // Assert
        Assert.That(length, Is.EqualTo(expected: 1));
    }

    [Test]
    public void InsertMap()
    {
        // Arrange
        var (doc, map) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        map.Insert(
            transaction, "value", Input.Map(
                new Dictionary<string, Input>
                {
                    { "value1", Input.Long(value: 2469L) },
                    { "value2", Input.Long(value: -420L) }
                }));
        var length = map.Length(transaction);
        transaction.Commit();

        // Assert
        Assert.That(length, Is.EqualTo(expected: 1));
    }

    [Test]
    public void InsertXmlElement()
    {
        // Arrange
        var (doc, map) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        map.Insert(transaction, "value", Input.XmlElement("person"));
        var length = map.Length(transaction);
        transaction.Commit();

        // Assert
        Assert.That(length, Is.EqualTo(expected: 1));
    }

    [Test]
    public void InsertXmlText()
    {
        // Arrange
        var (doc, map) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        map.Insert(transaction, "value", Input.XmlText("Lucas"));
        var length = map.Length(transaction);
        transaction.Commit();

        // Assert
        Assert.That(length, Is.EqualTo(expected: 1));
    }

    [Test]
    public void InsertDoc()
    {
        // Arrange
        var (doc, map) = ArrangeDoc();
        var subDoc = new Doc();

        // Act
        var transaction = doc.WriteTransaction();
        map.Insert(transaction, "sub-doc", Input.Doc(subDoc));
        var length = map.Length(transaction);
        transaction.Commit();

        // Assert
        Assert.That(length, Is.EqualTo(expected: 1));
    }

    [Test]
    public void InsertEqualTypeOnExistingKey()
    {
        // Arrange
        var (doc, map) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();

        map.Insert(transaction, "value", Input.Long(value: 2469L));
        map.Insert(transaction, "value", Input.Long(value: 420L));

        var value = map.Get(transaction, "value").Long;
        var length = map.Length(transaction);

        transaction.Commit();

        // Assert
        Assert.That(value, Is.EqualTo(expected: 420L));
        Assert.That(length, Is.EqualTo(expected: 1));
    }

    [Test]
    public void InsertDifferentTypeOnExistingKey()
    {
        // Arrange
        var (doc, map) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();

        map.Insert(transaction, "value", Input.Long(value: 2469L));
        map.Insert(transaction, "value", Input.String("Lucas"));

        var value = map.Get(transaction, "value");
        var length = map.Length(transaction);

        transaction.Commit();

        // Assert
        Assert.That(value.Tag, Is.EqualTo(OutputTag.String));
        Assert.That(value.String, Is.EqualTo("Lucas"));
        Assert.That(length, Is.EqualTo(expected: 1));
    }

    [Test]
    public void InsertNestedString()
    {
        // Arrange
        var (doc, map) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        var item1 = Input.Map(new Dictionary<string, Input>
        {
            { "id", Input.String(Guid.NewGuid().ToString()) },
            { "text", Input.String("Test") }
        });
        var item2 = Input.Map(new Dictionary<string, Input>
        {
            { "id", Input.String(Guid.NewGuid().ToString()) },
            { "text", Input.String("Describe the problem") }
        });

        var data = Input.Array(new[] { item1, item2 });
        map.Insert(transaction, "data", data);
        var length = map.Length(transaction);
        transaction.Commit();

        // Assert
        Assert.That(length, Is.EqualTo(expected: 1));
    }

    [Test]
    public void InsertNestedText()
    {
        // Arrange
        var (doc, map) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        var item1 = Input.Map(new Dictionary<string, Input>
        {
            { "id", Input.String(Guid.NewGuid().ToString()) },
            { "text", Input.Text("Test") }
        });
        var item2 = Input.Map(new Dictionary<string, Input>
        {
            { "id", Input.String(Guid.NewGuid().ToString()) },
            { "text", Input.Text("Describe the problem") }
        });

        var data = Input.Array(new[] { item1, item2 });
        map.Insert(transaction, "data", data);
        var length = map.Length(transaction);
        transaction.Commit();

        // Assert
        Assert.That(length, Is.EqualTo(expected: 1));
    }

    [Test]
    public void InsertNestedMap()
    {
        // Arrange
        var doc = new Doc();
        var map = doc.Map("map");

        // Act
        var transaction = doc.WriteTransaction();
        var innerMap = Input.Map(new Dictionary<string, Input>
        {
            { "text", Input.String("Nested data") }
        });
        var outerMap = Input.Map(new Dictionary<string, Input>
        {
            { "innerMap", innerMap }
        });
        map.Insert(transaction, "outerMap", outerMap);
        var length = map.Length(transaction);
        transaction.Commit();

        // Assert
        Assert.That(length, Is.EqualTo(expected: 1));
    }

    private (Doc, Map) ArrangeDoc()
    {
        var doc = new Doc();
        var map = doc.Map("map");

        return (doc, map);
    }
}
