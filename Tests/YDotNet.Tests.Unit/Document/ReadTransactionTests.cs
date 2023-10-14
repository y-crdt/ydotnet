using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.Document;

public class ReadTransactionTests
{
    [Test]
    public void ReadTransaction()
    {
        // Arrange
        var doc = new Doc();

        // Act
        var transaction = doc.WriteTransaction();

        // Assert
        Assert.That(transaction.Handle, Is.GreaterThan(nint.Zero));
    }

    [Test]
    public void MultipleReadTransactionsAllowed()
    {
        // Arrange
        var doc = new Doc();

        // Discard this transaction because it won't be used.
        doc.ReadTransaction();

        // Assert
        Assert.That(doc.ReadTransaction(), Is.Not.Null);
        Assert.That(doc.ReadTransaction(), Is.Not.Null);
    }

    [Test]
    public void GetRootMapWithOpenTransactionNotAllowed()
    {
        // Arrange
        var doc = new Doc();
        var map = doc.Map("Map");

        // Keep the transaction open.
        map.Length(doc.ReadTransaction());

        // Assert
        Assert.Throws<YDotNetException>(() => doc.Map("Item"));
    }

    [Test]
    public void GetRootArrayWithOpenTransactionNotAllowed()
    {
        // Arrange
        var doc = new Doc();
        var map = doc.Map("Map");

        // Keep the transaction open.
        map.Length(doc.ReadTransaction());

        // Assert
        Assert.Throws<YDotNetException>(() => doc.Array("Item"));
    }

    [Test]
    public void GetRootTextWithOpenTransactionNotAllowed()
    {
        // Arrange
        var doc = new Doc();
        var map = doc.Map("Map");

        // Keep the transaction open.
        map.Length(doc.ReadTransaction());

        // Assert
        Assert.Throws<YDotNetException>(() => doc.Text("Item"));
    }

    [Test]
    public void GetRootXmlTextWithOpenTransactionNotAllowed()
    {
        // Arrange
        var doc = new Doc();
        var map = doc.Map("Map");

        // Keep the transaction open.
        map.Length(doc.ReadTransaction());

        // Assert
        Assert.Throws<YDotNetException>(() => doc.XmlText("XmlText"));
    }

    [Test]
    public void GetRootXmlElementWithOpenTransactionNotAllowed()
    {
        // Arrange
        var doc = new Doc();
        var map = doc.Map("Map");

        // Keep the transaction open.
        map.Length(doc.ReadTransaction());

        // Assert
        Assert.Throws<YDotNetException>(() => doc.XmlElement("XmlElement"));
    }
}
