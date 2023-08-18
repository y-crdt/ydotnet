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
        var parentXmlElement = doc.XmlElement("xml-element");

        // TODO [LSViana] Check with the team why the event can't be generated for root nodes.
        var transaction = doc.WriteTransaction();
        var xmlElement = parentXmlElement.InsertElement(transaction, index: 0, "color");
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
        xmlElement.Unobserve(subscription);

        transaction = doc.WriteTransaction();
        xmlElement.InsertText(transaction, index: 1);
        transaction.Commit();

        // Assert
        Assert.That(called, Is.EqualTo(expected: 1));
    }
}
