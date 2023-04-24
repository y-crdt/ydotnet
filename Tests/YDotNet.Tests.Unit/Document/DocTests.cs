using System.Runtime.InteropServices;
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

    [Test]
    public void GetIdOnActive()
    {
        // Arrange
        var doc = new Doc();

        // Act
        var id = doc.Id;

        // Assert
        Assert.That(id, Is.GreaterThan(expected: 0));
    }

    [Test]
    public void GetIdOnDisposed()
    {
        // Arrange
        var doc = new Doc();
        doc.Dispose();

        // Act and Assert
        Assert.Throws<SEHException>(
            () =>
            {
                var _ = doc.Id;
            });
    }
}
