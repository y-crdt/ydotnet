using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.XmlElements;

public class IterateTests
{
    [Test]
    public void IteratesOnEmpty()
    {
        // Arrange
        var doc = new Doc();
        var xmlElement = doc.XmlElement("xml-element");

        // Act
        var transaction = doc.ReadTransaction();
        var iterator = xmlElement.Iterate(transaction);
        var values = iterator.ToArray();
        transaction.Commit();

        // Assert
        Assert.That(values, Is.Empty);
    }

    [Test]
    public void IteratesOnSingleItem()
    {
        // Arrange
        var doc = new Doc();
        var xmlElement = doc.XmlElement("xml-element");

        var transaction = doc.WriteTransaction();
        xmlElement.InsertAttribute(transaction, "href", "https://lsviana.github.io/");
        transaction.Commit();

        // Act
        transaction = doc.ReadTransaction();
        var iterator = xmlElement.Iterate(transaction);
        var values = iterator.ToArray();
        transaction.Commit();

        // Assert
        Assert.That(values.Length, Is.EqualTo(expected: 1));
        Assert.That(values.First(x => x.Key == "href").Value, Is.EqualTo("https://lsviana.github.io/"));
    }

    [Test]
    public void IteratesOnMultiItem()
    {
        // Arrange
        var doc = new Doc();
        var xmlElement = doc.XmlElement("xml-element");

        var transaction = doc.WriteTransaction();
        xmlElement.InsertAttribute(transaction, "href", "https://lsviana.github.io/");
        xmlElement.InsertAttribute(transaction, "as", "document");
        xmlElement.InsertAttribute(transaction, "rel", "preload");
        transaction.Commit();

        // Act
        transaction = doc.ReadTransaction();
        var iterator = xmlElement.Iterate(transaction);
        var values = iterator.ToArray();
        transaction.Commit();

        // Assert
        Assert.That(values.Length, Is.EqualTo(expected: 3));
        Assert.That(values.First(x => x.Key == "href").Value, Is.EqualTo("https://lsviana.github.io/"));
        Assert.That(values.First(x => x.Key == "as").Value, Is.EqualTo("document"));
        Assert.That(values.First(x => x.Key == "rel").Value, Is.EqualTo("preload"));
    }
}
