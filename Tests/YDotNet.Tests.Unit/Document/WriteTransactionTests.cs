using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.Document;

public class WriteTransactionTests
{
    [Test]
    public void WriteTransaction()
    {
        // Arrange
        var doc = new Doc();

        // Act
        var transaction = doc.WriteTransaction();

        // Assert
        Assert.That(transaction.Handle, Is.GreaterThan(nint.Zero));
    }

    [Test]
    public void MultipleWriteTransactionsNotAllowed()
    {
        // Arrange
        var doc = new Doc();

        // Assert
        Assert.That(doc.WriteTransaction(), Is.Not.Null);
        Assert.Throws<YDotNetException>(() => doc.WriteTransaction());
    }

    [Test]
    [Ignore("Still buggy in y-crdt")]
    public void GetRootMapWithOpenTransactionNotAllowed()
    {
        // Arrange
        var doc = new Doc();
        var map = doc.Map("Map");

        // Keep the transaction open.
        map.Length(doc.WriteTransaction());

        // Assert
        Assert.Throws<YDotNetException>(() => doc.Map("Item"));
    }

    [Test]
    [Ignore("Still buggy in y-crdt")]
    public void GetRootArrayWithOpenTransactionNotAllowed()
    {
        // Arrange
        var doc = new Doc();
        var map = doc.Map("Map");

        // Keep the transaction open.
        map.Length(doc.WriteTransaction());

        // Assert
        Assert.Throws<YDotNetException>(() => doc.Array("Item"));
    }

    [Test]
    [Ignore("Still buggy in y-crdt")]
    public void GetRootTextWithOpenTransactionNotAllowed()
    {
        // Arrange
        var doc = new Doc();
        var map = doc.Map("Map");

        // Keep the transaction open.
        map.Length(doc.WriteTransaction());

        // Assert
        Assert.Throws<YDotNetException>(() => doc.Text("Item"));
    }

    [Test]
    [Ignore("Still buggy in y-crdt")]
    public void GetRootXmlTextWithOpenTransactionNotAllowed()
    {
        // Arrange
        var doc = new Doc();
        var map = doc.Map("Map");

        // Keep the transaction open.
        map.Length(doc.WriteTransaction());

        // Assert
        Assert.Throws<YDotNetException>(() => doc.XmlText("XmlText"));
    }

    [Test]
    [Ignore("Still buggy in y-crdt")]
    public void GetRootXmlElementWithOpenTransactionNotAllowed()
    {
        // Arrange
        var doc = new Doc();
        var map = doc.Map("Map");

        // Keep the transaction open.
        map.Length(doc.WriteTransaction());

        // Assert
        Assert.Throws<YDotNetException>(() => doc.XmlElement("XmlElement"));
    }
}
