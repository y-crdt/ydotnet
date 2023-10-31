using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.XmlTexts;

public class UnobserveTests
{
    [Test]
    public void TriggersWhenXmlTextChangedUntilUnobserved()
    {
        // Arrange
        var doc = new Doc();
        var xmlText = doc.XmlText("xml-element");

        var called = 0;
        var subscription = xmlText.Observe(_ => called++);

        // Act
        var transaction = doc.WriteTransaction();
        xmlText.Insert(transaction, index: 0, "Lucas");
        transaction.Commit();

        // Assert
        Assert.That(called, Is.EqualTo(expected: 1));

        // Act
        subscription.Dispose();

        transaction = doc.WriteTransaction();
        xmlText.Insert(transaction, index: 1, " Viana");
        transaction.Commit();

        // Assert
        Assert.That(called, Is.EqualTo(expected: 1));
    }
}
