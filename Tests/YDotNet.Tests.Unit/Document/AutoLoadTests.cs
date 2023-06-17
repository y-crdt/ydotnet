using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.Document;

public class AutoLoadTests
{
    [Test]
    public void AutoLoad()
    {
        // Arrange
        var doc = new Doc();

        // Act
        var autoLoad = doc.AutoLoad;

        // Assert
        Assert.That(autoLoad, Is.False, "The default value for Doc.AutoLoad should be false.");
    }
}
