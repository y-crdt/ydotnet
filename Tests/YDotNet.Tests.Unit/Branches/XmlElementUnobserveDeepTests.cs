using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.Branches;

public class XmlElementUnobserveDeepTests
{
    [Test]
    public void TriggersWhenMapChangedUntilUnobserved()
    {
        // Arrange
        var doc = new Doc();
        var xmlElement = doc.XmlElement("xml-element");
        var called = 0;
        var subscription = xmlElement.ObserveDeep(_ => called++);

        // Act
        var transaction = doc.WriteTransaction();
        xmlElement.InsertText(transaction, index: 0);
        transaction.Commit();

        // Assert
        Assert.That(called, Is.EqualTo(expected: 1));

        // Act
        subscription.Dispose();

        transaction = doc.WriteTransaction();
        xmlElement.InsertText(transaction, index: 0);
        transaction.Commit();

        // Assert
        Assert.That(called, Is.EqualTo(expected: 1));
    }
}
