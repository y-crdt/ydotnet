using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.Transactions;

public class SnapshotTests
{
    [Test]
    public void ReadOnly()
    {
        // Arrange
        var doc = ArrangeDoc();

        // Act
        var snapshot = doc.ReadTransaction().Snapshot();

        // Assert
        Assert.That(snapshot, Is.Not.Null);
        Assert.That(snapshot.Length, Is.InRange(from: 7, to: 8));
    }

    [Test]
    public void ReadWrite()
    {
        // Arrange
        var doc = ArrangeDoc();

        // Act
        var snapshot = doc.WriteTransaction().Snapshot();

        // Assert
        Assert.That(snapshot, Is.Not.Null);
        Assert.That(snapshot.Length, Is.InRange(from: 7, to: 8));
    }

    private static Doc ArrangeDoc()
    {
        var doc = new Doc();
        var text = doc.Text("name");
        var transaction = doc.WriteTransaction();

        text.Insert(transaction, index: 0, "Lucas");
        transaction.Commit();

        return doc;
    }
}
