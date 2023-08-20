using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.XmlTexts;

public class StringTests
{
    [Test]
    public void StringIsInitiallyEmpty()
    {
        // Arrange
        var doc = new Doc();
        var xmlText = doc.XmlText("xml-text");

        // Act
        var transaction = doc.ReadTransaction();
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
        var xmlText = doc.XmlText("xml-text");

        // Act
        var transaction = doc.WriteTransaction();
        xmlText.Insert(transaction, index: 0, "Lucas Viana");
        var text = xmlText.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("Lucas Viana"));
    }

    [Test]
    [Ignore("Waiting for the necessary methods to be implemented.")]
    public void StringShowsInsertedAttribute()
    {
    }

    [Test]
    [Ignore("Waiting for the necessary methods to be implemented.")]
    public void StringShowsInsertedTextAndAttribute()
    {
    }

    [Test]
    [Ignore("Waiting for the necessary methods to be implemented.")]
    public void StringDoesNotShowInsertedEmbed()
    {
    }
}
