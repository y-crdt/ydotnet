using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.Transactions;

public class PendingTests
{
    [Test]
    public void NoPendingDataOnEmptyDoc()
    {
        // Arrange
        var doc = new Doc();
        using var transaction = doc.ReadTransaction();

        // Assert
        Assert.That(transaction.PendingUpdate(), Is.Null);
        Assert.That(transaction.PendingDeleteSet(), Is.Null);
    }

    [Test]
    public void PendingUpdateWhenDependencyIsMissing()
    {
        // Arrange
        var (first, second) = ArrangeDependentUpdates();
        var receiver = new Doc();

        // Act
        using (var transaction = receiver.WriteTransaction())
        {
            transaction.ApplyV1(second);
        }

        // Assert
        using (var transaction = receiver.ReadTransaction())
        {
            var pending = transaction.PendingUpdate();

            Assert.That(pending, Is.Not.Null);
            Assert.That(pending!.Update, Is.Not.Empty);
            Assert.That(pending.Missing.State, Is.Not.Empty);
            Assert.That(pending.Missing.State.Values, Has.All.GreaterThan(0u));
        }

        // Act
        using (var transaction = receiver.WriteTransaction())
        {
            transaction.ApplyV1(first);
        }

        // Assert
        using (var transaction = receiver.ReadTransaction())
        {
            Assert.That(transaction.PendingUpdate(), Is.Null);
        }
    }

    [Test]
    public void PendingDeleteSetWhenDeletedContentIsMissing()
    {
        // Arrange
        var sender = new Doc();
        var text = sender.Text("name");
        var updates = new List<byte[]>();
        using var subscription = sender.ObserveUpdatesV1(e => updates.Add(e.Update));

        using (var transaction = sender.WriteTransaction())
        {
            text.Insert(transaction, index: 0, "a");
        }

        using (var transaction = sender.WriteTransaction())
        {
            text.Insert(transaction, index: 1, "b");
        }

        using (var transaction = sender.WriteTransaction())
        {
            text.RemoveRange(transaction, index: 1, length: 1);
        }

        // The receiver only knows about "a", so the deletion of "b" cannot be applied.
        var receiver = new Doc();
        using (var transaction = receiver.WriteTransaction())
        {
            transaction.ApplyV1(updates[0]);
            transaction.ApplyV1(updates[2]);
        }

        // Assert
        using (var transaction = receiver.ReadTransaction())
        {
            var pendingDeleteSet = transaction.PendingDeleteSet();

            Assert.That(pendingDeleteSet, Is.Not.Null);
            Assert.That(pendingDeleteSet!.Ranges, Is.Not.Empty);
        }
    }

    private static (byte[] First, byte[] Second) ArrangeDependentUpdates()
    {
        var sender = new Doc();
        var text = sender.Text("name");

        byte[] first;
        using (var transaction = sender.WriteTransaction())
        {
            text.Insert(transaction, index: 0, "Lucas");
            first = transaction.StateDiffV1(stateVector: null);
        }

        byte[] stateVector;
        using (var transaction = sender.ReadTransaction())
        {
            stateVector = transaction.StateVectorV1();
        }

        byte[] second;
        using (var transaction = sender.WriteTransaction())
        {
            text.Insert(transaction, index: 5, " Cannon");
            second = transaction.StateDiffV1(stateVector);
        }

        return (first, second);
    }
}
