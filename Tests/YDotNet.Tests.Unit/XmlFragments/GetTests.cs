using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Types.XmlFragments;

namespace YDotNet.Tests.Unit.XmlFragments;

public class GetTests
{
    [Test]
    public void GetOutsideOfValidBounds()
    {
        // Arrange
        var doc = new Doc();
        var xmlFragment = doc.XmlFragment("xml-fragment");

        // Act
        var transaction = doc.WriteTransaction();
        var value = xmlFragment.Get(transaction, index: 1);
        transaction.Commit();

        // Assert
        Assert.That(value, Is.Null);
    }

    [Test]
    public void GetXmlText()
    {
        // Arrange
        var (doc, xmlFragment) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        var output = xmlFragment.Get(transaction, index: 1);
        transaction.Commit();

        // Assert
        Assert.That(output.XmlText, Is.Not.Null);
    }

    [Test]
    public void GetXmlElement()
    {
        // Arrange
        var (doc, xmlFragment) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        var output = xmlFragment.Get(transaction, index: 2);
        transaction.Commit();

        // Assert
        Assert.That(output.XmlElement, Is.Not.Null);
    }

    private (Doc, XmlFragment) ArrangeDoc()
    {
        var doc = new Doc();
        var xmlFragment = doc.XmlFragment("xml-fragment");

        // Act
        var transaction = doc.WriteTransaction();
        xmlFragment.InsertText(transaction, index: 0);
        xmlFragment.InsertText(transaction, index: 1);
        xmlFragment.InsertElement(transaction, index: 2, "color");
        xmlFragment.InsertText(transaction, index: 3);
        xmlFragment.InsertElement(transaction, index: 4, "border");
        transaction.Commit();

        return (doc, xmlFragment);
    }
}
