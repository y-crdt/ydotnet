using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.XmlFragments;

public class ChildLengthTests
{
    [Test]
    public void InitialLengthIsZero()
    {
        // Arrange
        var doc = new Doc();
        var xmlFragment = doc.XmlFragment("xml-fragment");

        // Act
        var transaction = doc.ReadTransaction();
        var childLength = xmlFragment.ChildLength(transaction);
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
        xmlFragment.InsertElement(transaction, index: 0, "xml-element-child-1");
        xmlFragment.InsertElement(transaction, index: 1, "xml-element-child-2");
        xmlFragment.InsertText(transaction, index: 2);
        xmlFragment.InsertText(transaction, index: 3);
        transaction.Commit();

        // Act
        transaction = doc.ReadTransaction();
        var childLength = xmlFragment.ChildLength(transaction);
        transaction.Commit();

        // Assert
        Assert.That(childLength, Is.EqualTo(expected: 4));
    }
}
