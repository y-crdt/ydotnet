using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.Document;

public class ObserveUpdatesV2
{
    [Test]
    public void TriggersWhenTransactionIsCommittedUntilUnobserve()
    {
        // Arrange
        var doc = new Doc();

        byte[]? data = null;
        var called = 0;

        var text = doc.Text("country");
        var subscription = doc.ObserveUpdatesV2(
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
        Assert.That(data, Has.Length.InRange(from: 33, to: 42));

        // Act
        data = null;
        transaction = doc.WriteTransaction();
        text.Insert(transaction, index: 0, "Great ");
        transaction.Commit();

        // Assert
        Assert.That(called, Is.EqualTo(expected: 2));
        Assert.That(data, Is.Not.Null);
        Assert.That(data, Has.Length.InRange(from: 26, to: 35));

        // Act
        data = null;
        subscription.Dispose();

        transaction = doc.WriteTransaction();
        text.Insert(transaction, index: 0, "The ");
        transaction.Commit();

        // Assert
        Assert.That(called, Is.EqualTo(expected: 2));
        Assert.That(data, Is.Null);
    }
}
