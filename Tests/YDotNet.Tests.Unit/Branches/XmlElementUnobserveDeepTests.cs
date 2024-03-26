using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;

namespace YDotNet.Tests.Unit.Branches;

public class XmlElementUnobserveDeepTests
{
    [Test]
    public void TriggersWhenMapChangedUntilUnobserved()
    {
        // Arrange
        var doc = new Doc();
        var map = doc.Map("map");

        var transaction = doc.WriteTransaction();
        map.Insert(transaction, "xml-element", Input.XmlElement("xml-element"));
        var xmlElement = map.Get(transaction, "xml-element").XmlElement;
        transaction.Commit();

        var called = 0;
        var subscription = xmlElement.ObserveDeep(_ => called++);

        // Act
        transaction = doc.WriteTransaction();
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
