using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.XmlTexts;

public class CreateTests
{
    [Test]
    public void Create()
    {
        // Arrange
        var doc = new Doc();

        // Act
        var xmlText = doc.XmlText("xml-text");

        // Assert
        Assert.That(xmlText.Handle, Is.GreaterThan(nint.Zero));
    }
}
