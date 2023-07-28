using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.Maps;

public class MapTests
{
    [Test]
    public void Create()
    {
        // Arrange
        var doc = new Doc();

        // Act
        var map = doc.Map("map");

        // Assert
        Assert.That(map.Handle, Is.GreaterThan(nint.Zero));
    }
}
