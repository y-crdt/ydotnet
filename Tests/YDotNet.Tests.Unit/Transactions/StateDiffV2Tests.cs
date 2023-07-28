using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.Transactions;

public class StateDiffV2Tests
{
    [Test]
    public void ReadOnly()
    {
        // Arrange
        var senderDoc = ArrangeSenderDoc();
        var receiverDoc = new Doc();

        // Act
        var stateVector = receiverDoc.ReadTransaction().StateVectorV1();
        var stateDiff = senderDoc.ReadTransaction().StateDiffV2(stateVector);

        // Assert
        Assert.That(stateDiff, Is.Not.Null);
        Assert.That(stateDiff.Length, Is.EqualTo(expected: 33));
    }

    [Test]
    public void ReadWrite()
    {
        // Arrange
        var senderDoc = ArrangeSenderDoc();
        var receiverDoc = new Doc();

        // Act
        var stateVector = receiverDoc.ReadTransaction().StateVectorV1();
        var stateDiff = senderDoc.WriteTransaction().StateDiffV2(stateVector);

        // Assert
        Assert.That(stateDiff, Is.Not.Null);
        Assert.That(stateDiff.Length, Is.EqualTo(expected: 33));
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
