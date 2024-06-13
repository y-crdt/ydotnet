using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.XmlFragments;

public class UnobserveTests
{
    [Test]
    public void TriggersWhenXmlElementChangedUntilUnobserved()
    {
        // Arrange
        var doc = new Doc();
        var xmlFragment = doc.XmlFragment("xml-fragment");

        var called = 0;
        var subscription = xmlFragment.Observe(_ => called++);

        // Act
        var transaction = doc.WriteTransaction();
        xmlFragment.InsertText(transaction, index: 0);
        transaction.Commit();

        // Assert
        Assert.That(called, Is.EqualTo(expected: 1));

        // Act
        subscription.Dispose();

        transaction = doc.WriteTransaction();
        xmlFragment.InsertText(transaction, index: 1);
        transaction.Commit();

        // Assert
        Assert.That(called, Is.EqualTo(expected: 1));
    }
}
