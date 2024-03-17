using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Types.XmlFragments;

namespace YDotNet.Tests.Unit.XmlFragments;

public class RemoveRangeTests
{
    [Test]
    public void RemoveEmptyRangeOnEmptyElement()
    {
        // Arrange
        var doc = new Doc();
        var xmlFragment = doc.XmlFragment("xml-fragment");

        // Act
        var transaction = doc.WriteTransaction();
        xmlFragment.RemoveRange(transaction, index: 0, length: 0);
        var childLength = xmlFragment.ChildLength(transaction);
        transaction.Commit();

        // Assert
        Assert.That(childLength, Is.EqualTo(expected: 0));
    }

    [Test]
    public void RemoveEmptyRangeOnFilledElement()
    {
        // Arrange
        var (doc, xmlFragment) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        xmlFragment.RemoveRange(transaction, index: 2, length: 0);
        var childLength = xmlFragment.ChildLength(transaction);
        transaction.Commit();

        // Assert
        Assert.That(childLength, Is.EqualTo(expected: 5));
    }

    [Test]
    public void RemoveSingleItemRange()
    {
        // Arrange
        var (doc, xmlFragment) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        xmlFragment.RemoveRange(transaction, index: 3, length: 1);
        var childLength = xmlFragment.ChildLength(transaction);
        transaction.Commit();

        // Assert
        Assert.That(childLength, Is.EqualTo(expected: 4));
    }

    [Test]
    public void RemoveMultipleItemsRange()
    {
        // Arrange
        var (doc, xmlFragment) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        xmlFragment.RemoveRange(transaction, index: 2, length: 3);
        var childLength = xmlFragment.ChildLength(transaction);
        transaction.Commit();

        // Assert
        Assert.That(childLength, Is.EqualTo(expected: 2));
    }

    [Test]
    public void RemoveAllItemsAtOnce()
    {
        // Arrange
        var (doc, xmlFragment) = ArrangeDoc();

        // Act
        var transaction = doc.WriteTransaction();
        xmlFragment.RemoveRange(transaction, index: 0, length: 5);
        var childLength = xmlFragment.ChildLength(transaction);
        transaction.Commit();

        // Assert
        Assert.That(childLength, Is.EqualTo(expected: 0));
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
