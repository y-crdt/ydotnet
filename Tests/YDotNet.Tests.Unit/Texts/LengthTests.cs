using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.Texts;

public class LengthTests
{
    [Test]
    public void LengthIsInitiallyZero()
    {
        // Arrange
        var doc = new Doc();
        var text = doc.Text("name");

        // Act
        var transaction = doc.ReadTransaction();
        var length = text.Length(transaction);

        // Assert
        Assert.That(length, Is.EqualTo(expected: 0));
    }

    [Test]
    public void LengthIsCorrectForAlphanumericCharacters()
    {
        // Arrange
        var doc = new Doc();
        var text = doc.Text("name");

        // Act
        var transaction = doc.WriteTransaction();
        text.Insert(transaction, index: 0, "Lucas");
        var length = text.Length(transaction);

        // Assert
        Assert.That(length, Is.EqualTo(expected: 5));
    }

    [Test]
    public void LengthIsCorrectForAlphanumericAndEmojiCharacters()
    {
        // Arrange
        var doc = new Doc();
        var text = doc.Text("name");

        // Act
        var transaction = doc.WriteTransaction();
        text.Insert(transaction, index: 0, "Earth 🌎");
        var length = text.Length(transaction);

        // Assert
        Assert.That(length, Is.EqualTo(expected: 7));
    }
}
