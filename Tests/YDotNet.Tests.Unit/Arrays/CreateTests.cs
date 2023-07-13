using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.Arrays;

public class CreateTests
{
    [Test]
    public void Create()
    {
        // Arrange
        var doc = new Doc();

        // Act
        var array = doc.Array("array");

        // Assert
        Assert.That(array.Handle, Is.GreaterThan(nint.Zero));
        Assert.That(array.Length, Is.EqualTo(expected: 0));
    }
}
