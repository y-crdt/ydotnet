using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.XmlTexts;

public class IterateTests
{
    [Test]
    public void IteratesOnEmpty()
    {
        // Arrange
        var doc = new Doc();
        var xmlFragment = doc.XmlFragment("xml-fragment");

        var transaction = doc.WriteTransaction();
        var xmlText = xmlFragment.InsertText(transaction, index: 0);
        transaction.Commit();

        // Act
        transaction = doc.ReadTransaction();
        var iterator = xmlText.Iterate(transaction);
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
        var xmlFragment = doc.XmlFragment("xml-fragment");

        var transaction = doc.WriteTransaction();
        var xmlText = xmlFragment.InsertText(transaction, index: 0);
        xmlText.InsertAttribute(transaction, "href", "https://lsviana.github.io/");
        transaction.Commit();

        // Act
        transaction = doc.ReadTransaction();
        var iterator = xmlText.Iterate(transaction);
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
        var xmlFragment = doc.XmlFragment("xml-fragment");

        var transaction = doc.WriteTransaction();
        var xmlText = xmlFragment.InsertText(transaction, index: 0);
        xmlText.InsertAttribute(transaction, "href", "https://lsviana.github.io/");
        xmlText.InsertAttribute(transaction, "as", "document");
        xmlText.InsertAttribute(transaction, "rel", "preload");
        transaction.Commit();

        // Act
        transaction = doc.ReadTransaction();
        var iterator = xmlText.Iterate(transaction);
        var values = iterator.ToArray();
        transaction.Commit();

        // Assert
        Assert.That(values.Length, Is.EqualTo(expected: 3));
        Assert.That(values.First(x => x.Key == "href").Value, Is.EqualTo("https://lsviana.github.io/"));
        Assert.That(values.First(x => x.Key == "as").Value, Is.EqualTo("document"));
        Assert.That(values.First(x => x.Key == "rel").Value, Is.EqualTo("preload"));
    }

    [Test]
    public void ResetRestartsTheEnumeration()
    {
        // Arrange
        var doc = new Doc();
        var xmlFragment = doc.XmlFragment("xml-fragment");

        var setup = doc.WriteTransaction();
        var xmlText = xmlFragment.InsertText(setup, index: 0);
        xmlText.InsertAttribute(setup, "href", "https://lsviana.github.io/");
        xmlText.InsertAttribute(setup, "rel", "preload");
        setup.Commit();

        var transaction = doc.ReadTransaction();
        using var enumerator = xmlText.Iterate(transaction).GetEnumerator();
        var firstPass = new List<string>();
        while (enumerator.MoveNext())
        {
            firstPass.Add(enumerator.Current.Key);
        }

        // Act
        enumerator.Reset();
        var secondPass = new List<string>();
        while (enumerator.MoveNext())
        {
            secondPass.Add(enumerator.Current.Key);
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
        var xmlText = xmlFragment.InsertText(setup, index: 0);
        xmlText.InsertAttribute(setup, "href", "https://lsviana.github.io/");
        xmlText.InsertAttribute(setup, "rel", "preload");
        setup.Commit();

        var transaction = doc.ReadTransaction();
        var enumerator = xmlText.Iterate(transaction).GetEnumerator();
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

        var setup = doc.WriteTransaction();
        var xmlText = xmlFragment.InsertText(setup, index: 0);
        setup.Commit();

        var transaction = doc.ReadTransaction();
        transaction.Commit();

        // Act and Assert
        Assert.Throws<ObjectDisposedException>(() => xmlText.Iterate(transaction));
    }
}
