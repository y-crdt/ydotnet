using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.XmlElements;

public class GetAttributeTests
{
    [Test]
    public void GetAttributeThatDoesNotExistReturnsNull()
    {
        // Arrange
        var doc = new Doc();
        var xmlFragment = doc.XmlFragment("xml-fragment");

        var transaction = doc.WriteTransaction();
        var xmlElement = xmlFragment.InsertElement(transaction, index: 0, "xml-element");
        transaction.Commit();

        // Act
        transaction = doc.ReadTransaction();
        var value = xmlElement.GetAttribute(transaction, "href");
        transaction.Commit();

        // Assert
        Assert.That(value, Is.Null);
    }

    [Test]
    public void GetAttributeThatExistsAndIsEmptyWorks()
    {
        // Arrange
        var doc = new Doc();
        var xmlFragment = doc.XmlFragment("xml-fragment");

        var transaction = doc.WriteTransaction();
        var xmlElement = xmlFragment.InsertElement(transaction, index: 0, "xml-element");
        transaction.Commit();

        // Act
        transaction = doc.WriteTransaction();
        xmlElement.InsertAttribute(transaction, "href", string.Empty);
        transaction.Commit();

        // Act
        transaction = doc.ReadTransaction();
        var value = xmlElement.GetAttribute(transaction, "href");
        transaction.Commit();

        // Assert
        Assert.That(value, Is.EqualTo(string.Empty));
    }

    [Test]
    public void GetAttributeThatExistsAndIsFilledWorks()
    {
        // Arrange
        var doc = new Doc();
        var xmlFragment = doc.XmlFragment("xml-fragment");

        var transaction = doc.WriteTransaction();
        var xmlElement = xmlFragment.InsertElement(transaction, index: 0, "xml-element");
        transaction.Commit();

        // Act
        transaction = doc.WriteTransaction();
        xmlElement.InsertAttribute(transaction, "href", "https://lsviana.github.io/");
        transaction.Commit();

        // Act
        transaction = doc.ReadTransaction();
        var value = xmlElement.GetAttribute(transaction, "href");
        transaction.Commit();

        // Assert
        Assert.That(value, Is.EqualTo("https://lsviana.github.io/"));

        // Act
        transaction = doc.WriteTransaction();
        xmlElement.InsertAttribute(transaction, "href", "https://github.com/LSViana/ydotnet/");
        value = xmlElement.GetAttribute(transaction, "href");
        transaction.Commit();

        // Assert
        Assert.That(value, Is.EqualTo("https://github.com/LSViana/ydotnet/"));
    }
}
