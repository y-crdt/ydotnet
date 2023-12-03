using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.Document;

public class IdTests
{
    [Test]
    public void IsGreaterOrEqualToZeroByDefault()
    {
        // Arrange
        var doc = new Doc();

        // Act
        var id = doc.Id;

        // Assert
        Assert.That(id, Is.GreaterThanOrEqualTo(expected: 0));
    }
}
