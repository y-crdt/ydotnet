using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.XmlElements;

public class ChildLengthTests
{
    [Test]
    public void InitialLengthIsZero()
    {
        // Arrange
        var doc = new Doc();
        var xmlFragment = doc.XmlFragment("xml-fragment");

        var transaction = doc.WriteTransaction();
        var xmlElement = xmlFragment.InsertElement(transaction, index: 0, "xml-element");
        transaction.Commit();

        // Act
        transaction = doc.ReadTransaction();
        var childLength = xmlElement.ChildLength(transaction);
        transaction.Commit();

        // Assert
        Assert.That(childLength, Is.EqualTo(expected: 0));
    }

    [Test]
    public void ChildLengthMatchesAmountOfChildrenAdded()
    {
        // Arrange
        var doc = new Doc();
        var xmlFragment = doc.XmlFragment("xml-fragment");

        var transaction = doc.WriteTransaction();
        var xmlElement = xmlFragment.InsertElement(transaction, index: 0, "xml-element");
        xmlElement.InsertElement(transaction, index: 0, "xml-element-child-1");
        xmlElement.InsertElement(transaction, index: 1, "xml-element-child-2");
        xmlElement.InsertText(transaction, index: 2);
        transaction.Commit();

        // Act
        transaction = doc.ReadTransaction();
        var childLength = xmlElement.ChildLength(transaction);
        transaction.Commit();

        // Assert
        Assert.That(childLength, Is.EqualTo(expected: 3));
    }
}
