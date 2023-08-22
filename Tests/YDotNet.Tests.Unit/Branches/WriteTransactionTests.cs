using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Types.Branches;

namespace YDotNet.Tests.Unit.Branches;

public class WriteTransactionTests
{
    [Test]
    public void StartSingleWriteTransaction()
    {
        // Arrange
        var doc = new Doc();
        Branch branch = doc.Text("branch");

        // Act
        var transaction = branch.WriteTransaction();

        // Assert
        Assert.That(transaction, Is.Not.Null);
    }

    [Test]
    public void StartMultipleWriteTransactions()
    {
        // Arrange
        var doc = new Doc();
        Branch branch = doc.Array("branch");

        // Act
        var transaction1 = branch.WriteTransaction();
        var transaction2 = branch.WriteTransaction();

        // Assert
        Assert.That(transaction1, Is.Not.Null);
        Assert.That(transaction2, Is.Null);
    }

    [Test]
    public void StartWriteTransactionWhileReadTransactionIsOpen()
    {
        // Arrange
        var doc = new Doc();
        Branch branch = doc.Array("branch");

        // Act
        var readTransaction = branch.ReadTransaction();
        var writeTransaction = branch.WriteTransaction();

        // Assert
        Assert.That(readTransaction, Is.Not.Null);
        Assert.That(writeTransaction, Is.Null);
    }

    [Test]
    public void StartWriteTransactionWhileDocumentReadTransactionIsOpen()
    {
        // Arrange
        var doc = new Doc();
        Branch branch = doc.Array("branch");

        // Act
        var readTransaction = doc.ReadTransaction();
        var writeTransaction = branch.WriteTransaction();

        // Assert
        Assert.That(readTransaction, Is.Not.Null);
        Assert.That(writeTransaction, Is.Null);
    }

    [Test]
    public void StartWriteTransactionWhileDocumentWriteTransactionIsOpen()
    {
        // Arrange
        var doc = new Doc();
        Branch branch = doc.Array("branch");

        // Act
        var transaction1 = doc.WriteTransaction();
        var transaction2 = branch.WriteTransaction();

        // Assert
        Assert.That(transaction1, Is.Not.Null);
        Assert.That(transaction2, Is.Null);
    }
}
