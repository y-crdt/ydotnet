using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.Document;

public class ReadTransactionTests
{
    [Test]
    public void ReadTransaction()
    {
        // Arrange
        var doc = new Doc();

        // Act
        var transaction = doc.WriteTransaction();

        // Assert
        Assert.That(transaction.Handle, Is.GreaterThan(nint.Zero));
    }

    [Test]
    public void MultipleReadTransactionsAllowed()
    {
        // Arrange
        var doc = new Doc();

        // Discard this transaction because it won't be used.
        doc.ReadTransaction();

        // Assert
        Assert.That(doc.ReadTransaction(), Is.Not.Null);
        Assert.That(doc.ReadTransaction(), Is.Not.Null);
    }
}
