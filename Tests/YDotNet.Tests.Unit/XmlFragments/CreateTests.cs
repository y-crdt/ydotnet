using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.XmlFragments;

public class CreateTests
{
    [Test]
    public void Create()
    {
        // Arrange
        var doc = new Doc();

        // Act
        var xmlFragment = doc.XmlFragment("xml-fragment");

        // Assert
        Assert.That(xmlFragment.Handle, Is.GreaterThan(nint.Zero));
    }
}
