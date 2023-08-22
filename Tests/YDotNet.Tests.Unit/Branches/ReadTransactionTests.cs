using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Types.Branches;

namespace YDotNet.Tests.Unit.Branches;

public class ReadTransactionTests
{
    [Test]
    public void StartReadTransaction()
    {
        // Arrange
        var doc = new Doc();
        Branch branch = doc.Text("branch");

        // Act
        var transaction = branch.ReadTransaction();

        // Assert
        Assert.That(transaction, Is.Not.Null);
    }

    [Test]
    public void StartReadTransactionWhileDocumentReadTransactionIsOpen()
    {
        // Arrange
        var doc = new Doc();
        Branch branch = doc.Text("branch");

        // Act
        var documentTransaction = doc.ReadTransaction();
        var branchTransaction = branch.ReadTransaction();

        // Assert
        Assert.That(documentTransaction, Is.Not.Null);
        Assert.That(branchTransaction, Is.Not.Null);
    }

    [Test]
    [Ignore("Implement this test when Branch.WriteTransaction() is available.")]
    public void StartReadTransactionWhileWriteTransactionIsOpen()
    {
        // TODO [LSViana] Implement this test when Branch.WriteTransaction() is available.
    }

    [Test]
    public void StartReadTransactionWhileDocumentWriteTransactionIsOpen()
    {
        // Arrange
        var doc = new Doc();
        Branch branch = doc.Text("branch");

        // Act
        var documentTransaction = doc.WriteTransaction();
        var branchTransaction = branch.ReadTransaction();

        // Assert
        Assert.That(documentTransaction, Is.Not.Null);
        Assert.That(branchTransaction, Is.Null);
    }
}
