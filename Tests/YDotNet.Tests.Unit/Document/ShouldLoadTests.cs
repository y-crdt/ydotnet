using System.Runtime.InteropServices;
using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.Document;

public class ShouldLoadTests
{
    [Test]
    public void Active()
    {
        // Arrange
        var doc = new Doc();

        // Act
        var shouldLoad = doc.ShouldLoad;

        // Assert
        Assert.That(shouldLoad, Is.True, "The default value for Doc.ShouldLoad should be true.");
    }

    [Test]
    public void Disposed()
    {
        // Arrange
        var doc = new Doc();
        doc.Dispose();

        // Act and Assert
        Assert.Throws<SEHException>(
            () =>
            {
                var _ = doc.ShouldLoad;
            });
    }
}
