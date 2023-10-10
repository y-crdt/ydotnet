using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.Document;

public class WriteTransactionTests
{
    [Test]
    public void WriteTransaction()
    {
        // Arrange
        var doc = new Doc();

        // Act
        var transaction = doc.WriteTransaction();

        // Assert
        Assert.That(transaction.Handle, Is.GreaterThan(nint.Zero));
    }

    [Test]
    public void MultipleWriteTransactionsNotAllowed()
    {
        // Arrange
        var doc = new Doc();

        // Assert
        Assert.That(doc.WriteTransaction(), Is.Not.Null);
        Assert.Throws<YDotNetException>(() => doc.WriteTransaction());
    }
}
