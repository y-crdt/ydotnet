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
        var xmlFragment = doc.XmlFragment("xml-fragment");

        // Act
        var transaction = doc.WriteTransaction();
        var xmlText = xmlFragment.InsertText(transaction, index: 0);
        transaction.Commit();

        // Assert
        Assert.That(xmlText.Handle, Is.GreaterThan(nint.Zero));
    }
}
