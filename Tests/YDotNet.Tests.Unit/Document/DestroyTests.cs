using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.Document;

public class DestroyTests
{
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
