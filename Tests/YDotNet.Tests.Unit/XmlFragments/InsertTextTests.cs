using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.XmlFragments;

public class InsertTextTests
{
    [Test]
    public void InsertSingleText()
    {
        // Arrange
        var doc = new Doc();
        var xmlFragment = doc.XmlFragment("xml-fragment");

        // Act
        var transaction = doc.WriteTransaction();
        var xmlText = xmlFragment.InsertText(transaction, index: 0);
        var childLength = xmlFragment.ChildLength(transaction);
        transaction.Commit();

        // Assert
        Assert.That(xmlText.Handle, Is.Not.EqualTo(nint.Zero));
        Assert.That(childLength, Is.EqualTo(expected: 1));
    }

    [Test]
    public void InsertMultipleTexts()
    {
        // Arrange
        var doc = new Doc();
        var xmlFragment = doc.XmlFragment("xml-fragment");

        // Act
        var transaction = doc.WriteTransaction();
        xmlFragment.InsertText(transaction, index: 0);
        xmlFragment.InsertText(transaction, index: 0);
        xmlFragment.InsertText(transaction, index: 0);
        var childLength = xmlFragment.ChildLength(transaction);
        transaction.Commit();

        // Assert
        Assert.That(childLength, Is.EqualTo(expected: 3));
    }
}
