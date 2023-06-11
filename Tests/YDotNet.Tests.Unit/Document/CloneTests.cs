using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.Document;

public class CloneTests
{
    [Test]
    public void Clone()
    {
        // Arrange
        var doc = new Doc();

        // Act
        var clone = doc.Clone();

        // Assert
        Assert.That(clone.Handle, Is.Not.EqualTo(nint.Zero));
        Assert.That(clone.Handle, Is.Not.EqualTo(doc.Handle));
        Assert.That(clone.Id, Is.EqualTo(doc.Id));
    }
}
