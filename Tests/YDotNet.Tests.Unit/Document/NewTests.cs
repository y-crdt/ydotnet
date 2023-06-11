using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.Document;

public class NewTests
{
    [Test]
    public void Create()
    {
        // Arrange and Act
        var doc = new Doc();

        // Assert
        Assert.That(doc.Handle, Is.GreaterThan(nint.Zero));
    }
}
