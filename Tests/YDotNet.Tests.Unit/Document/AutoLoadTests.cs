using System.Runtime.InteropServices;
using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.Document;

public class AutoLoadTests
{
    [Test]
    public void Active()
    {
        // Arrange
        var doc = new Doc();

        // Act
        var autoLoad = doc.AutoLoad;

        // Assert
        Assert.That(autoLoad, Is.False, "The default value for Doc.AutoLoad should be false.");
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
                var _ = doc.AutoLoad;
            });
    }
}
