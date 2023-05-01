using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.Document;

public class ObserveUpdatesV1
{
    [Test]
    public void TriggersOnceWhenTransactionIsCommitted()
    {
        // Arrange
        var doc = new Doc();

        byte[] data = null;
        var called = 0;

        var text = doc.Text("country");
        doc.ObserveUpdatesV1(
            e =>
            {
                called++;
                data = e.Update;
            });

        // Act
        var transaction = doc.WriteTransaction();
        text.Insert(transaction, index: 0, "Brazil");
        transaction.Commit();

        // Assert
        Assert.That(called, Is.EqualTo(expected: 1));
        Assert.That(data, Is.Not.Null);
        Assert.That(data, Has.Length.EqualTo(expected: 26));
    }
}
