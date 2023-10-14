using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Events;

namespace YDotNet.Tests.Unit.Document;

public class AfterTransactionTests
{
    [Test]
    public void TriggersWhenTransactionIsCommittedUntilUnobserve()
    {
        // Arrange
        var doc = new Doc();

        AfterTransactionEvent? afterTransactionEvent = null;
        var called = 0;

        var text = doc.Text("country");
        var subscription = doc.ObserveAfterTransaction(
            e =>
            {
                called++;
                afterTransactionEvent = e;
            });

        // Act
        var transaction = doc.WriteTransaction();
        text.Insert(transaction, index: 0, "Brazil");
        transaction.Commit();

        // Assert
        Assert.That(called, Is.EqualTo(expected: 1));
        Assert.That(afterTransactionEvent, Is.Not.Null);
        Assert.That(afterTransactionEvent.AfterState, Is.Not.Null);
        Assert.That(afterTransactionEvent.BeforeState, Is.Not.Null);
        Assert.That(afterTransactionEvent.DeleteSet, Is.Not.Null);

        // Act
        afterTransactionEvent = null;
        transaction = doc.WriteTransaction();
        text.Insert(transaction, index: 0, "Great ");
        transaction.Commit();

        // Assert
        Assert.That(called, Is.EqualTo(expected: 2));
        Assert.That(afterTransactionEvent, Is.Not.Null);
        Assert.That(afterTransactionEvent.AfterState, Is.Not.Null);
        Assert.That(afterTransactionEvent.BeforeState, Is.Not.Null);
        Assert.That(afterTransactionEvent.DeleteSet, Is.Not.Null);

        // Act
        afterTransactionEvent = null;
        subscription.Dispose();

        transaction = doc.WriteTransaction();
        text.Insert(transaction, index: 0, "The ");
        transaction.Commit();

        // Assert
        Assert.That(called, Is.EqualTo(expected: 2));
        Assert.That(afterTransactionEvent, Is.Null);
    }
}
