using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.Transactions;

public class StateVectorV1Tests
{
    [Test]
    public void ReadOnly()
    {
        // Arrange
        var doc = ArrangeDoc();

        // Act
        var stateVector = doc.ReadTransaction().StateVectorV1();

        // Assert
        Assert.That(stateVector, Is.Not.Null);
        Assert.That(stateVector.Length, Is.InRange(from: 6, to: 7));
    }

    [Test]
    public void ReadWrite()
    {
        // Arrange
        var doc = ArrangeDoc();

        // Act
        var stateVector = doc.WriteTransaction().StateVectorV1();

        // Assert
        Assert.That(stateVector, Is.Not.Null);
        Assert.That(stateVector.Length, Is.InRange(from: 6, to: 7));
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
