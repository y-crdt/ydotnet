using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;

namespace YDotNet.Tests.Unit.XmlTexts;

public class RemoveRangeTests
{
    [Test]
    public void RemoveNothing()
    {
        // Arrange
        var doc = new Doc();
        var xmlFragment = doc.XmlFragment("xml-fragment");

        var transaction = doc.WriteTransaction();
        var xmlText = xmlFragment.InsertText(transaction, index: 0);
        transaction.Commit();

        // Act
        transaction = doc.WriteTransaction();
        xmlText.RemoveRange(transaction, index: 0, length: 0);
        var text = xmlText.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo(string.Empty));
    }

    [Test]
    public void RemovePartialText()
    {
        // Arrange
        var doc = new Doc();
        var xmlFragment = doc.XmlFragment("xml-fragment");

        var transaction = doc.WriteTransaction();
        var xmlText = xmlFragment.InsertText(transaction, index: 0);
        transaction.Commit();

        // Act
        transaction = doc.WriteTransaction();
        xmlText.Insert(transaction, index: 0, "Lucas");
        transaction.Commit();

        // Act
        transaction = doc.WriteTransaction();
        xmlText.RemoveRange(transaction, index: 2, length: 1);
        var text = xmlText.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("Luas"));
    }

    [Test]
    public void RemoveFullText()
    {
        // Arrange
        var doc = new Doc();
        var xmlFragment = doc.XmlFragment("xml-fragment");

        var transaction = doc.WriteTransaction();
        var xmlText = xmlFragment.InsertText(transaction, index: 0);
        transaction.Commit();

        // Act
        transaction = doc.WriteTransaction();
        xmlText.Insert(transaction, index: 0, "Lucas");
        transaction.Commit();

        // Act
        transaction = doc.WriteTransaction();
        xmlText.RemoveRange(transaction, index: 2, length: 1);
        var text = xmlText.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("Luas"));
    }

    [Test]
    public void RemoveBooleanEmbed()
    {
        // Arrange
        var doc = new Doc();
        var xmlFragment = doc.XmlFragment("xml-fragment");

        var transaction = doc.WriteTransaction();
        var xmlText = xmlFragment.InsertText(transaction, index: 0);
        transaction.Commit();

        // Act
        transaction = doc.WriteTransaction();
        xmlText.Insert(transaction, index: 0, "Lucas");
        xmlText.InsertEmbed(transaction, index: 2, Input.Boolean(value: true));
        transaction.Commit();

        // Act
        transaction = doc.WriteTransaction();
        xmlText.RemoveRange(transaction, index: 4, length: 1);
        var text = xmlText.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("Lutruecs"));

        // Act
        transaction = doc.WriteTransaction();
        xmlText.RemoveRange(transaction, index: 2, length: 1);
        text = xmlText.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("Lucs"));
    }

    [Test]
    public void RemoveObjectEmbed()
    {
        // Arrange
        var doc = new Doc();
        var xmlFragment = doc.XmlFragment("xml-fragment");

        var transaction = doc.WriteTransaction();
        var xmlText = xmlFragment.InsertText(transaction, index: 0);
        transaction.Commit();

        // Act
        transaction = doc.WriteTransaction();
        xmlText.Insert(transaction, index: 0, "Lucas");
        xmlText.InsertEmbed(
            transaction, index: 2, Input.Object(
                new Dictionary<string, Input>
                {
                    { "italic", Input.Boolean(value: true) }
                }));
        transaction.Commit();

        // Act
        transaction = doc.WriteTransaction();
        xmlText.RemoveRange(transaction, index: 4, length: 1);
        var text = xmlText.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("Lu{italic: true}cs"));

        // Act
        transaction = doc.WriteTransaction();
        xmlText.RemoveRange(transaction, index: 2, length: 1);
        text = xmlText.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("Lucs"));
    }
}
