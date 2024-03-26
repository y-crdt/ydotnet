using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.Branches;

public class XmlFragmentUnobserveDeepTests
{
    [Test]
    public void TriggersWhenChangedUntilUnobserved()
    {
        // Arrange
        var doc = new Doc();
        var xmlFragment = doc.XmlFragment("xml-fragment");

        var called = 0;
        var subscription = xmlFragment.ObserveDeep(_ => called++);

        // Act
        var transaction = doc.WriteTransaction();
        xmlFragment.InsertText(transaction, index: 0);
        transaction.Commit();

        // Assert
        Assert.That(called, Is.EqualTo(expected: 1));

        // Act
        subscription.Dispose();

        transaction = doc.WriteTransaction();
        xmlFragment.InsertText(transaction, index: 0);
        transaction.Commit();

        // Assert
        Assert.That(called, Is.EqualTo(expected: 1));
    }
}
