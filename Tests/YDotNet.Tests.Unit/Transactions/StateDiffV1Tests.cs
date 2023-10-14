using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.Transactions;

public class StateDiffV1Tests
{
    [Test]
    public void Null()
    {
        // Arrange
        var senderDoc = ArrangeSenderDoc();

        // Act
        var stateDiff = senderDoc.ReadTransaction().StateDiffV1(stateVector: null);

        // Assert
        Assert.That(stateDiff, Is.Not.Null);
        Assert.That(stateDiff, Has.Length.InRange(from: 22, to: 26));
    }

    [Test]
    public void ReadOnly()
    {
        // Arrange
        var senderDoc = ArrangeSenderDoc();
        var receiverDoc = new Doc();

        // Act
        var stateVector = receiverDoc.ReadTransaction().StateVectorV1();
        var stateDiff = senderDoc.ReadTransaction().StateDiffV1(stateVector);

        // Assert
        Assert.That(stateDiff, Is.Not.Null);
        Assert.That(stateDiff.Length, Is.GreaterThan(expected: 19));
    }

    [Test]
    public void ReadWrite()
    {
        // Arrange
        var senderDoc = ArrangeSenderDoc();
        var receiverDoc = new Doc();

        // Act
        var stateVector = receiverDoc.ReadTransaction().StateVectorV1();
        var stateDiff = senderDoc.WriteTransaction().StateDiffV1(stateVector);

        // Assert
        Assert.That(stateDiff, Is.Not.Null);
        Assert.That(stateDiff.Length, Is.GreaterThan(expected: 19));
    }

    private static Doc ArrangeSenderDoc()
    {
        var doc = new Doc();
        var text = doc.Text("name");
        var transaction = doc.WriteTransaction();

        text.Insert(transaction, index: 0, "Lucas");
        transaction.Commit();

        return doc;
    }
}
