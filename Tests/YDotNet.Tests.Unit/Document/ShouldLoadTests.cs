using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.Document;

public class ShouldLoadTests
{
    [Test]
    public void ShouldLoad()
    {
        // Arrange
        var doc = new Doc();

        // Act
        var shouldLoad = doc.ShouldLoad;

        // Assert
        Assert.That(shouldLoad, Is.True, "The default value for Doc.ShouldLoad should be true.");
    }
}
