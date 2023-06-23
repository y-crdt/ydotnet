using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Transactions;

namespace YDotNet.Tests.Unit.Transactions;

public class ApplyV2Tests
{
    [Test]
    public void ApplyAndCheckChanges()
    {
        // Arrange
        var senderDoc = ArrangeSenderDoc();
        var receiverDoc = new Doc();
        var receiverText = receiverDoc.Text("name");
        var receiverTransaction = receiverDoc.WriteTransaction();
        var senderTransaction = senderDoc.ReadTransaction();

        // Act
        var stateDiff = senderTransaction.StateDiffV2(receiverTransaction.StateVectorV1());
        var result = receiverTransaction.ApplyV2(stateDiff);
        var text = receiverText.String(receiverTransaction);

        // Assert
        Assert.That(result, Is.EqualTo(TransactionUpdateResult.Ok));
        Assert.That(text, Is.EqualTo("Lucas"));
    }

    // TODO [LSViana] Add transaction to ensure that read-only transactions can't use `Apply`.

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
