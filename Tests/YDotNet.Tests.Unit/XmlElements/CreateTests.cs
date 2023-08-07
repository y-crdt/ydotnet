using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.XmlElements;

public class CreateTests
{
    [Test]
    public void Create()
    {
        // Arrange
        var doc = new Doc();

        // Act
        var xmlElement = doc.XmlElement("xml-element");

        // Assert
        Assert.That(xmlElement.Handle, Is.GreaterThan(nint.Zero));
    }
}
