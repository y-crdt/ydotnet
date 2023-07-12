using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;
using YDotNet.Document.Transactions;
using YDotNet.Document.Types;

namespace YDotNet.Tests.Unit.Maps;

public class InsertTests
{
    [Test]
    public void InsertBoolean()
    {
        // Arrange
        var (map, transaction) = ArrangeDoc();
        var value1 = true;
        var value2 = false;

        // Assert
        Assert.That(map.Length(transaction), Is.EqualTo(expected: 0));

        // Act
        map.Insert(transaction, "value1", Input.Boolean(value1));
        map.Insert(transaction, "value2", Input.Boolean(value2));

        // Assert
        Assert.That(map.Length(transaction), Is.EqualTo(expected: 2));
    }

    [Test]
    public void InsertDouble()
    {
        // Arrange
        var (map, transaction) = ArrangeDoc();
        var value1 = 24.69;
        var value2 = -4.20;

        // Assert
        Assert.That(map.Length(transaction), Is.EqualTo(expected: 0));

        // Act
        map.Insert(transaction, "value1", Input.Double(value1));
        map.Insert(transaction, "value2", Input.Double(value2));

        // Assert
        Assert.That(map.Length(transaction), Is.EqualTo(expected: 2));
    }

    [Test]
    public void InsertLong()
    {
        // Arrange
        var (map, transaction) = ArrangeDoc();
        var value1 = 2469L;
        var value2 = -420L;

        // Assert
        Assert.That(map.Length(transaction), Is.EqualTo(expected: 0));

        // Act
        map.Insert(transaction, "value1", Input.Long(value1));
        map.Insert(transaction, "value2", Input.Long(value2));

        // Assert
        Assert.That(map.Length(transaction), Is.EqualTo(expected: 2));
    }

    [Test]
    public void InsertString()
    {
        // Arrange
        var (map, transaction) = ArrangeDoc();
        var name = "Lucas";

        // Assert
        Assert.That(map.Length(transaction), Is.EqualTo(expected: 0));

        // Act
        map.Insert(transaction, "name", Input.String(name));

        // Assert
        Assert.That(map.Length(transaction), Is.EqualTo(expected: 1));
    }

    [Test]
    public void InsertBytes()
    {
        // Arrange
        var (map, transaction) = ArrangeDoc();
        var value = new byte[] { 2, 4, 6, 9 };

        // Assert
        Assert.That(map.Length(transaction), Is.EqualTo(expected: 0));

        // Act
        map.Insert(transaction, "value", Input.Bytes(value));

        // Assert
        Assert.That(map.Length(transaction), Is.EqualTo(expected: 1));
    }

    [Test]
    public void InsertCollection()
    {
        // Arrange
        var (map, transaction) = ArrangeDoc();

        // Assert
        Assert.That(map.Length(transaction), Is.EqualTo(expected: 0));

        // Act
        map.Insert(
            transaction, "value", Input.Collection(
                new[]
                {
                    Input.Long(value: 2469L),
                    Input.Long(value: -420L)
                }));

        // Assert
        Assert.That(map.Length(transaction), Is.EqualTo(expected: 1));
    }

    [Test]
    public void InsertObject()
    {
        // Arrange
        var (map, transaction) = ArrangeDoc();

        // Assert
        Assert.That(map.Length(transaction), Is.EqualTo(expected: 0));

        // Act
        map.Insert(
            transaction, "value", Input.Object(
                new Dictionary<string, Input>
                {
                    { "value1", Input.Long(value: 2469L) },
                    { "value2", Input.Long(value: -420L) }
                }));

        // Assert
        Assert.That(map.Length(transaction), Is.EqualTo(expected: 1));
    }

    [Test]
    public void InsertNull()
    {
        // Arrange
        var (map, transaction) = ArrangeDoc();

        // Assert
        Assert.That(map.Length(transaction), Is.EqualTo(expected: 0));

        // Act
        map.Insert(transaction, "value", Input.Null());

        // Assert
        Assert.That(map.Length(transaction), Is.EqualTo(expected: 1));
    }

    [Test]
    public void InsertUndefined()
    {
        // Arrange
        var (map, transaction) = ArrangeDoc();

        // Assert
        Assert.That(map.Length(transaction), Is.EqualTo(expected: 0));

        // Act
        map.Insert(transaction, "value", Input.Undefined());

        // Assert
        Assert.That(map.Length(transaction), Is.EqualTo(expected: 1));
    }

    [Test]
    [Ignore("To be implemented.")]
    public void InsertText()
    {
    }

    [Test]
    public void InsertArray()
    {
        // Arrange
        var (map, transaction) = ArrangeDoc();

        // Assert
        Assert.That(map.Length(transaction), Is.EqualTo(expected: 0));

        // Act
        map.Insert(
            transaction, "value", Input.Array(
                new[]
                {
                    Input.Long(value: 2469L),
                    Input.Long(value: -420L)
                }));

        // Assert
        Assert.That(map.Length(transaction), Is.EqualTo(expected: 1));
    }

    [Test]
    public void InsertMap()
    {
        // Arrange
        var (map, transaction) = ArrangeDoc();

        // Assert
        Assert.That(map.Length(transaction), Is.EqualTo(expected: 0));

        // Act
        map.Insert(
            transaction, "value", Input.Map(
                new Dictionary<string, Input>
                {
                    { "value1", Input.Long(value: 2469L) },
                    { "value2", Input.Long(value: -420L) }
                }));

        // Assert
        Assert.That(map.Length(transaction), Is.EqualTo(expected: 1));
    }

    [Test]
    [Ignore("To be implemented.")]
    public void InsertXmlElement()
    {
    }

    [Test]
    [Ignore("To be implemented.")]
    public void InsertXmlText()
    {
    }

    [Test]
    public void InsertDoc()
    {
        // Arrange
        var (map, transaction) = ArrangeDoc();
        var subDoc = new Doc();

        // Assert
        Assert.That(map.Length(transaction), Is.EqualTo(expected: 0));

        // Act
        map.Insert(transaction, "sub-doc", Input.Doc(subDoc));

        // Assert
        Assert.That(map.Length(transaction), Is.EqualTo(expected: 1));
    }

    [Test]
    [Ignore("To be implemented.")]
    public void InsertEqualTypeOnExistingKey()
    {
    }

    [Test]
    [Ignore("To be implemented.")]
    public void InsertDifferentTypeOnExistingKey()
    {
    }

    [Test]
    [Ignore("To be implemented.")]
    public void InsertSameInstanceOnMultipleKeys()
    {
    }

    private (Map?, Transaction?) ArrangeDoc()
    {
        var doc = new Doc();
        var map = doc.Map("map");
        var transaction = doc.WriteTransaction();

        return (map, transaction);
    }
}
