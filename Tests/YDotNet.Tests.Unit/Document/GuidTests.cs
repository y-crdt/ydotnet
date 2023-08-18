using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Options;

namespace YDotNet.Tests.Unit.Document;

public class GuidTests
{
    [Test]
    public void RandomGuidByDefault()
    {
        // Arrange
        var doc = new Doc();

        // Act
        var guid = doc.Guid;

        // Assert
        Assert.That(guid, Is.Not.EqualTo(string.Empty));
    }

    [Test]
    public void SpecialCharacters()
    {
        // Arrange
        var doc = new Doc(
            new DocOptions
            {
                Guid = "shark-🦈"
            });

        // Act
        var guid = doc.Guid;

        // Assert
        Assert.That(guid, Is.EqualTo("shark-🦈"));
    }
}
