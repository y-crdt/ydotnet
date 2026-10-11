using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Transactions;

namespace YDotNet.Tests.Unit.Transactions;

public class ApplyErrorTests
{
    [Test]
    public void EmptyUpdateReportsEndOfStream()
    {
        // Arrange
        var doc = new Doc();
        using var transaction = doc.WriteTransaction();

        // Act
        var result = transaction.ApplyV1(System.Array.Empty<byte>());

        // Assert
        Assert.That(result, Is.EqualTo(TransactionUpdateResult.EndOfStream));
    }

    [Test]
    public void TruncatedUpdateReportsEndOfStream()
    {
        // Arrange
        var doc = new Doc();
        using var transaction = doc.WriteTransaction();

        // Act
        var result = transaction.ApplyV1(new byte[] { 1, 1, 1 });

        // Assert
        Assert.That(result, Is.EqualTo(TransactionUpdateResult.EndOfStream));
    }

    [Test]
    public void OversizedVariableIntegerReportsIntegerOutOfBounds()
    {
        // Arrange
        var doc = new Doc();
        using var transaction = doc.WriteTransaction();
        var update = Enumerable.Repeat((byte)0xFF, count: 12).ToArray();

        // Act
        var result = transaction.ApplyV1(update);

        // Assert
        Assert.That(result, Is.EqualTo(TransactionUpdateResult.IntegerOutOfBounds));
    }

    [Test]
    public void UnknownContentTypeReportsUnexpectedValue()
    {
        // Arrange
        // One client, one block with an unassigned content type (0x1F). This is the same decoding failure that
        // updates containing array moves (removed in yrs 0.27) cause.
        var update = new byte[] { 1, 1, 1, 0, 0x1F, 1, 1, 0x61, 0 };
        var doc = new Doc();
        using var transaction = doc.WriteTransaction();

        // Act
        var result = transaction.ApplyV1(update);

        // Assert
        Assert.That(result, Is.EqualTo(TransactionUpdateResult.UnexpectedValue));
    }

    [Test]
    public void FailedUpdateDoesNotChangeDocument()
    {
        // Arrange
        var doc = new Doc();
        var text = doc.Text("a");
        var update = new byte[] { 1, 1, 1, 0, 0x1F, 1, 1, 0x61, 0 };

        // Act
        using (var transaction = doc.WriteTransaction())
        {
            transaction.ApplyV1(update);
        }

        // Assert
        using (var transaction = doc.ReadTransaction())
        {
            Assert.That(text.String(transaction), Is.Empty);
        }
    }
}
