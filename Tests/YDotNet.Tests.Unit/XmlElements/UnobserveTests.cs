using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.XmlElements;

public class UnobserveTests
{
    [Test]
    public void TriggersWhenXmlElementChangedUntilUnobserved()
    {
        // Arrange
        var doc = new Doc();
        var xmlFragment = doc.XmlFragment("xml-fragment");

        var transaction = doc.WriteTransaction();
        var xmlElement = xmlFragment.InsertElement(transaction, index: 0, "xml-element");
        transaction.Commit();

        var called = 0;
        var subscription = xmlElement.Observe(_ => called++);

        // Act
        transaction = doc.WriteTransaction();
        xmlElement.InsertText(transaction, index: 0);
        transaction.Commit();

        // Assert
        Assert.That(called, Is.EqualTo(expected: 1));

        // Act
        subscription.Dispose();

        transaction = doc.WriteTransaction();
        xmlElement.InsertText(transaction, index: 1);
        transaction.Commit();

        // Assert
        Assert.That(called, Is.EqualTo(expected: 1));
    }
}
