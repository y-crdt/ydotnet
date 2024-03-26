using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;

namespace YDotNet.Tests.Unit.XmlTexts;

public class StringTests
{
    [Test]
    public void StringIsInitiallyEmpty()
    {
        // Arrange
        var doc = new Doc();
        var xmlFragment = doc.XmlFragment("xml-fragment");

        var transaction = doc.WriteTransaction();
        var xmlText = xmlFragment.InsertText(transaction, index: 0);
        transaction.Commit();

        // Act
        transaction = doc.ReadTransaction();
        var text = xmlText.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo(string.Empty));
    }

    [Test]
    public void StringShowsInsertedText()
    {
        // Arrange
        var doc = new Doc();
        var xmlFragment = doc.XmlFragment("xml-fragment");

        var transaction = doc.WriteTransaction();
        var xmlText = xmlFragment.InsertText(transaction, index: 0);
        transaction.Commit();

        // Act
        transaction = doc.WriteTransaction();
        xmlText.Insert(transaction, index: 0, "Lucas Viana");
        var text = xmlText.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("Lucas Viana"));
    }

    [Test]
    public void StringDoesNotShowInsertedAttribute()
    {
        // Arrange
        var doc = new Doc();
        var xmlFragment = doc.XmlFragment("xml-fragment");

        var transaction = doc.WriteTransaction();
        var xmlText = xmlFragment.InsertText(transaction, index: 0);
        transaction.Commit();

        // Act
        transaction = doc.WriteTransaction();
        xmlText.Insert(transaction, index: 0, "Lucas Viana");
        xmlText.InsertAttribute(transaction, "color", "red");
        var text = xmlText.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("Lucas Viana"));
    }

    [Test]
    public void StringShowsInsertedEmbed()
    {
        // Arrange
        var doc = new Doc();
        var xmlFragment = doc.XmlFragment("xml-fragment");

        var transaction = doc.WriteTransaction();
        var xmlText = xmlFragment.InsertText(transaction, index: 0);
        transaction.Commit();

        // Act
        transaction = doc.WriteTransaction();
        xmlText.Insert(transaction, index: 0, "Lucas Viana");
        xmlText.InsertEmbed(transaction, index: 3, Input.Boolean(value: true));
        xmlText.InsertEmbed(transaction, index: 8, Input.Long(value: 2469L));
        xmlText.InsertEmbed(
            transaction, index: 11, Input.Object(
                new Dictionary<string, Input>
                {
                    { "color", Input.Bytes(new byte[] { 255, 79, 113 }) }
                }));
        var text = xmlText.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("Luctrueas V2469ia{color: 0xff4f71}na"));
    }
}
