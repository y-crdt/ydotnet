using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Options;
using YDotNet.Document.Options;
using YDotNet.Document.State;

namespace YDotNet.Tests.Unit.Document;

public class StateVectorTests
{
    [Test]
    public void ReportsClocksForSingleClient()
    {
        // Arrange
        var doc = new Doc(new DocOptions { Id = 1001 });
        var text = doc.Text("country");
        var states = new List<(StateVector Before, StateVector After)>();

        using var subscription = doc.ObserveAfterTransaction(e => states.Add((e.BeforeState, e.AfterState)));

        // Act
        using (var transaction = doc.WriteTransaction())
        {
            text.Insert(transaction, index: 0, "Brazil");
        }

        using (var transaction = doc.WriteTransaction())
        {
            text.Insert(transaction, index: 0, "Great ");
        }

        // Assert
        Assert.That(states, Has.Count.EqualTo(2));

        Assert.That(states[0].Before.State, Is.Empty);
        Assert.That(states[0].After.State, Is.EqualTo(new Dictionary<ulong, uint> { [doc.Id] = 6 }));

        Assert.That(states[1].Before.State, Is.EqualTo(new Dictionary<ulong, uint> { [doc.Id] = 6 }));
        Assert.That(states[1].After.State, Is.EqualTo(new Dictionary<ulong, uint> { [doc.Id] = 12 }));
    }

    [Test]
    public void ReportsDistinctClocksForMultipleClients()
    {
        // Arrange
        var sender = new Doc(new DocOptions { Id = 2002 });
        var senderText = sender.Text("country");

        using (var transaction = sender.WriteTransaction())
        {
            senderText.Insert(transaction, index: 0, "Brazil");
        }

        var receiver = new Doc(new DocOptions { Id = 3003 });
        var receiverText = receiver.Text("country");
        StateVector? after = null;

        using var subscription = receiver.ObserveAfterTransaction(e => after = e.AfterState);

        using (var transaction = receiver.WriteTransaction())
        {
            receiverText.Insert(transaction, index: 0, "Hi");
        }

        // Act
        byte[] diff;
        using (var transaction = sender.ReadTransaction())
        {
            diff = transaction.StateDiffV1(stateVector: null);
        }

        using (var transaction = receiver.WriteTransaction())
        {
            transaction.ApplyV1(diff);
        }

        // Assert
        Assert.That(after, Is.Not.Null);
        Assert.That(
            after!.State,
            Is.EqualTo(new Dictionary<ulong, uint> { [sender.Id] = 6, [receiver.Id] = 2 }));
    }
}
