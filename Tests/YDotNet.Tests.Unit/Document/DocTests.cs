using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.Document;

public class DocTests
{
    [Test]
    public void Create()
    {
        // Arrange and Act
        var doc = new Doc();

        // Assert
        Assert.That(doc.Handle, Is.GreaterThan(nint.Zero));
    }

    [Test]
    public void Dispose()
    {
        // Arrange
        var doc = new Doc();

        // Act
        doc.Dispose();

        // Assert
        Assert.That(doc.Handle, Is.EqualTo(nint.Zero));
    }
}
