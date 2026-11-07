using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;

namespace YDotNet.Tests.Unit.XmlFragments;

public class TreeWalkerTests
{
    [Test]
    public void WalksOnEmptyTree()
    {
        // Arrange
        var doc = new Doc();
        var xmlFragment = doc.XmlFragment("xml-fragment");

        // Act
        var transaction = doc.ReadTransaction();
        var xmlTreeWalker = xmlFragment.TreeWalker(transaction);
        transaction.Commit();

        // Assert
        Assert.That(xmlTreeWalker, Is.Not.Null);
        Assert.That(xmlTreeWalker.ToArray().Length, Is.EqualTo(expected: 0));
    }

    [Test]
    public void WalksOnTreeWithSingleLevelOfDepth()
    {
        // Arrange
        var doc = new Doc();
        var xmlFragment = doc.XmlFragment("xml-fragment");

        // Act
        var transaction = doc.WriteTransaction();
        xmlFragment.InsertText(transaction, index: 0);
        xmlFragment.InsertElement(transaction, index: 1, "color");
        xmlFragment.InsertText(transaction, index: 2);
        transaction.Commit();

        // Act
        transaction = doc.ReadTransaction();
        var xmlTreeWalker = xmlFragment.TreeWalker(transaction);

        // Assert
        var xmlNodes = xmlTreeWalker.ToArray();

        Assert.That(xmlTreeWalker, Is.Not.Null);
        Assert.That(xmlNodes.Length, Is.EqualTo(expected: 3));
        Assert.That(xmlNodes.ElementAt(index: 0).XmlText, Is.Not.Null);
        Assert.That(xmlNodes.ElementAt(index: 1).XmlElement, Is.Not.Null);
        Assert.That(xmlNodes.ElementAt(index: 1).XmlElement.Tag(transaction), Is.EqualTo("color"));
        Assert.That(xmlNodes.ElementAt(index: 2).XmlText, Is.Not.Null);

        transaction.Commit();
    }

    [Test]
    public void WalksOnTreeWithMultipleLevelsOfDepth()
    {
        // Arrange
        var doc = new Doc();
        var xmlFragment = doc.XmlFragment("xml-fragment");

        // Act
        var transaction = doc.WriteTransaction();
        xmlFragment.InsertText(transaction, index: 0);
        xmlFragment.InsertElement(transaction, index: 1, "color");
        var childXmlElement = xmlFragment.Get(transaction, index: 1).XmlElement;
        childXmlElement.InsertElement(transaction, index: 0, "alpha");
        childXmlElement.InsertElement(transaction, index: 1, "hex");
        childXmlElement.InsertText(transaction, index: 2);
        transaction.Commit();

        // Act
        transaction = doc.ReadTransaction();
        var xmlTreeWalker = xmlFragment.TreeWalker(transaction);

        // Assert
        var xmlNodes = xmlTreeWalker.ToArray();

        Assert.That(xmlTreeWalker, Is.Not.Null);
        Assert.That(xmlNodes.Length, Is.EqualTo(expected: 5));
        Assert.That(xmlNodes.ElementAt(index: 0).XmlText, Is.Not.Null);
        Assert.That(xmlNodes.ElementAt(index: 1).XmlElement, Is.Not.Null);
        Assert.That(xmlNodes.ElementAt(index: 1).XmlElement.Tag(transaction), Is.EqualTo("color"));
        Assert.That(xmlNodes.ElementAt(index: 2).XmlElement, Is.Not.Null);
        Assert.That(xmlNodes.ElementAt(index: 2).XmlElement.Tag(transaction), Is.EqualTo("alpha"));
        Assert.That(xmlNodes.ElementAt(index: 3).XmlElement, Is.Not.Null);
        Assert.That(xmlNodes.ElementAt(index: 3).XmlElement.Tag(transaction), Is.EqualTo("hex"));
        Assert.That(xmlNodes.ElementAt(index: 4).XmlText, Is.Not.Null);

        transaction.Commit();
    }

    [Test]
    public void ResetRestartsTheEnumeration()
    {
        // Arrange
        var doc = new Doc();
        var xmlFragment = doc.XmlFragment("xml-fragment");

        var setup = doc.WriteTransaction();
        xmlFragment.InsertText(setup, index: 0);
        xmlFragment.InsertElement(setup, index: 1, "color");
        setup.Commit();

        var transaction = doc.ReadTransaction();
        using var enumerator = xmlFragment.TreeWalker(transaction).GetEnumerator();
        var firstPass = new List<OutputTag>();
        while (enumerator.MoveNext())
        {
            firstPass.Add(enumerator.Current.Tag);
        }

        // Act
        enumerator.Reset();
        var secondPass = new List<OutputTag>();
        while (enumerator.MoveNext())
        {
            secondPass.Add(enumerator.Current.Tag);
        }

        transaction.Commit();

        // Assert
        Assert.That(firstPass, Has.Count.EqualTo(expected: 2));
        Assert.That(secondPass, Is.EqualTo(firstPass));
    }

    [Test]
    public void ResetThrowsAfterTransactionIsCommitted()
    {
        // Arrange
        var doc = new Doc();
        var xmlFragment = doc.XmlFragment("xml-fragment");

        var setup = doc.WriteTransaction();
        xmlFragment.InsertText(setup, index: 0);
        xmlFragment.InsertElement(setup, index: 1, "color");
        setup.Commit();

        var transaction = doc.ReadTransaction();
        var enumerator = xmlFragment.TreeWalker(transaction).GetEnumerator();
        transaction.Commit();

        // Act and Assert
        Assert.Throws<ObjectDisposedException>(() => enumerator.Reset());
    }

    [Test]
    public void ThrowsWhenTransactionIsCommitted()
    {
        // Arrange
        var doc = new Doc();
        var xmlFragment = doc.XmlFragment("xml-fragment");

        var transaction = doc.ReadTransaction();
        transaction.Commit();

        // Act and Assert
        Assert.Throws<ObjectDisposedException>(() => xmlFragment.TreeWalker(transaction));
    }
}
