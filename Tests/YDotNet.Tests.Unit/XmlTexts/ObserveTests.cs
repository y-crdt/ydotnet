using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Types.XmlTexts;

namespace YDotNet.Tests.Unit.XmlTexts;

public class ObserveTests
{
    [Test]
    public void ObserveHasTarget()
    {
        // Arrange
        var doc = new Doc();
        var xmlText = doc.XmlText("xml-text");

        XmlText? target = null;
        var subscription = xmlText.Observe(e => target = e.Target);

        // Act
        var transaction = doc.WriteTransaction();
        xmlText.Insert(transaction, index: 0, "Lucas");
        transaction.Commit();

        // Assert
        Assert.That(subscription.Id, Is.EqualTo(expected: 0L));
        Assert.That(target, Is.Not.Null);
        Assert.That(target.Handle, Is.Not.EqualTo(nint.Zero));
    }
}
