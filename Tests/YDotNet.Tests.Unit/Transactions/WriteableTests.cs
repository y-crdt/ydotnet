using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.Transactions;

public class WriteableTests
{
    [Test]
    public void ReadOnly()
    {
        // Arrange
        var doc = new Doc();

        // Act
        var transaction = doc.ReadTransaction();

        // Assert
        Assert.That(transaction.Writeable, Is.False);
    }

    [Test]
    public void ReadWrite()
    {
        // Arrange
        var doc = new Doc();

        // Act
        var transaction = doc.WriteTransaction();

        // Assert
        Assert.That(transaction.Writeable, Is.True);
    }
}
