using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Options;

namespace YDotNet.Tests.Unit.Document;

public class CollectionIdTests
{
    [Test]
    public void NullByDefault()
    {
        // Arrange
        var doc = new Doc();

        // Act
        var collectionId = doc.CollectionId;

        // Assert
        Assert.That(collectionId, Is.Null);
    }

    [Test]
    public void SpecialCharacters()
    {
        // Arrange
        var doc = new Doc(
            new DocOptions
            {
                CollectionId = "dragon-🐲"
            });

        // Act
        var collectionId = doc.CollectionId;

        // Assert
        Assert.That(collectionId, Is.EqualTo("dragon-🐲"));
    }
}
