using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;

namespace YDotNet.Tests.Unit.Branches;

public class XmlTextUnobserveDeepTests
{
    [Test]
    public void TriggersWhenChangedUntilUnobserved()
    {
        // Arrange
        var doc = new Doc();
        var map = doc.Map("map");

        var transaction = doc.WriteTransaction();
        map.Insert(transaction, "xml-text", Input.XmlText("xml-text"));
        var xmlText = map.Get(transaction, "xml-text").XmlText;
        transaction.Commit();

        var called = 0;
        var subscription = xmlText.ObserveDeep(_ => called++);

        // Act
        transaction = doc.WriteTransaction();
        xmlText.Insert(transaction, index: 0, "World");
        transaction.Commit();

        // Assert
        Assert.That(called, Is.EqualTo(expected: 1));

        // Act
        subscription.Dispose();

        transaction = doc.WriteTransaction();
        xmlText.Insert(transaction, index: 0, "Hello, ");
        transaction.Commit();

        // Assert
        Assert.That(called, Is.EqualTo(expected: 1));
    }
}
