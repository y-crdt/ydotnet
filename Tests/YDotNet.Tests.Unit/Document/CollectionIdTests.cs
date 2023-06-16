using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.Document;

public class CollectionIdTests
{
    [Test]
    public void Active()
    {
        // Arrange
        var doc = new Doc();

        // Act
        var collectionId = doc.CollectionId;

        // Assert
        Assert.That(collectionId, Is.Null);
    }
}
