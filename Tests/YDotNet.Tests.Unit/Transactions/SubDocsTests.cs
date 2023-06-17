using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.Transactions;

public class SubDocsTests
{
    [Test]
    public void ReadOnly()
    {
        // Arrange
        var (doc, subDocs) = ArrangeDoc(subDocs: 2);

        // Act
        var transactionSubDocs = doc.ReadTransaction().SubDocs();

        // Assert
        Assert.That(transactionSubDocs.Length, Is.EqualTo(expected: 2));
        Assert.That(transactionSubDocs.Select(x => x.Id), Is.EqualTo(subDocs.Select(x => x.Id)));
    }

    [Test]
    public void ReadWrite()
    {
        // Arrange
        var (doc, subDocs) = ArrangeDoc(subDocs: 3);

        // Act
        var transactionSubDocs = doc.WriteTransaction().SubDocs();

        // Assert
        Assert.That(transactionSubDocs.Length, Is.EqualTo(expected: 3));
        Assert.That(transactionSubDocs.Select(x => x.Id), Is.EqualTo(subDocs.Select(x => x.Id)));
    }

    private (Doc, Doc[]) ArrangeDoc(int subDocs)
    {
        var doc = new Doc();
        var map = doc.Map("sub-docs");
        var transaction = doc.WriteTransaction();
        var subDocuments = new Doc[subDocs];

        for (var i = 0; i < subDocs; i++)
        {
            subDocuments[i] = new Doc();
            map.Insert(transaction, $"sub-doc-{i - 1}", subDocuments[i]);
        }

        transaction.Commit();

        return (doc, subDocuments);
    }
}
