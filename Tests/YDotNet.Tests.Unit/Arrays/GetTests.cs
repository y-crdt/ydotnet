using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;
using Array = YDotNet.Document.Types.Arrays.Array;

namespace YDotNet.Tests.Unit.Arrays;

public class GetTests
{
    [Test]
    public void GetFromEmptyArray()
    {
        // Arrange
        var doc = new Doc();
        var array = doc.Array("array");

        // Act
        var transaction = doc.ReadTransaction();
        var output = array.Get(transaction, index: 0);
        transaction.Commit();

        // Assert
        Assert.That(output, Is.Null);
    }

    [Test]
    public void GetAtBeginning()
    {
        // Arrange
        var (doc, array) = ArrangeDoc();

        // Act
        var transaction = doc.ReadTransaction();
        var output = array.Get(transaction, index: 0);
        transaction.Commit();

        // Assert
        Assert.That(output, Is.Not.Null);
        Assert.That(output.Boolean, Is.True);
        Assert.That(output.Long, Is.Null);
    }

    [Test]
    public void GetAtMiddle()
    {
        // Arrange
        var (doc, array) = ArrangeDoc();

        // Act
        var transaction = doc.ReadTransaction();
        var output = array.Get(transaction, index: 1);
        transaction.Commit();

        // Assert
        Assert.That(output, Is.Not.Null);
        Assert.That(output.Undefined, Is.True);
        Assert.That(output.Long, Is.Null);
    }

    [Test]
    public void GetAtEnding()
    {
        // Arrange
        var (doc, array) = ArrangeDoc();

        // Act
        var transaction = doc.ReadTransaction();
        var output = array.Get(transaction, index: 2);
        transaction.Commit();

        // Assert
        Assert.That(output, Is.Not.Null);
        Assert.That(output.String, Is.EqualTo("Lucas"));
        Assert.That(output.Long, Is.Null);
    }

    [Test]
    public void GetMultipleTimesAtSameIndex()
    {
        // Arrange
        var (doc, array) = ArrangeDoc();

        // Act
        var transaction = doc.ReadTransaction();
        var output1 = array.Get(transaction, index: 0);
        var output2 = array.Get(transaction, index: 0);
        transaction.Commit();

        // Assert
        Assert.That(output1, Is.Not.Null);
        Assert.That(output1.Boolean, Is.True);
        Assert.That(output1.Long, Is.Null);

        Assert.That(output2, Is.Not.Null);
        Assert.That(output2.Boolean, Is.True);
        Assert.That(output2.Long, Is.Null);
    }

    [Test]
    public void GetOnInvalidIndex()
    {
        // Arrange
        var (doc, array) = ArrangeDoc();

        // Act
        var transaction = doc.ReadTransaction();
        var output = array.Get(transaction, index: 420);
        transaction.Commit();

        // Assert
        Assert.That(output, Is.Null);
    }

    private (Doc, Array) ArrangeDoc()
    {
        var doc = new Doc();
        var array = doc.Array("array");

        var transaction = doc.WriteTransaction();
        array.InsertRange(
            transaction, index: 0, new[]
            {
                Input.Boolean(value: true),
                Input.Undefined(),
                Input.String("Lucas")
            });
        transaction.Commit();

        return (doc, array);
    }
}
