using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Options;

namespace YDotNet.Tests.Unit.Transactions;

public class EncodeStateFromSnapshotV1Tests
{
    // TODO [LSViana] Check with the team if this is how this feature is supposed to be used.
    [Test]
    public void CreateAndApplySnapshot()
    {
        // Arrange
        var originDoc = ArrangeDoc(skipGarbageCollection: true);
        var originTransaction = originDoc.ReadTransaction();
        var snapshot = originTransaction.Snapshot();

        // Act
        var stateDiff = originTransaction.EncodeStateFromSnapshotV1(snapshot);

        // Assert
        Assert.That(stateDiff, Is.Not.Null);
        Assert.That(stateDiff.Length, Is.EqualTo(expected: 17));

        // Act
        var targetDoc = new Doc();
        var targetText = targetDoc.Text("name");
        var targetTransaction = targetDoc.WriteTransaction();

        targetTransaction.ApplyV1(stateDiff);

        // Assert
        Assert.That(targetText.String(targetTransaction), Is.EqualTo("Lucas"));
    }

    [Test]
    public void ReturnsNullWhenDocumentHasGarbageCollection()
    {
        // Arrange
        var originDoc = ArrangeDoc(skipGarbageCollection: false);
        var originTransaction = originDoc.ReadTransaction();
        var snapshot = originTransaction.Snapshot();

        // Act
        var stateDiff = originTransaction.EncodeStateFromSnapshotV1(snapshot);

        // Assert
        Assert.That(stateDiff, Is.Null);
    }

    private static Doc ArrangeDoc(bool skipGarbageCollection)
    {
        var doc = new Doc(
            new DocOptions
            {
                SkipGarbageCollection = skipGarbageCollection
            }
        );
        var text = doc.Text("name");
        var transaction = doc.WriteTransaction();

        text.Insert(transaction, index: 0, "Lucas");
        transaction.Commit();

        return doc;
    }
}
