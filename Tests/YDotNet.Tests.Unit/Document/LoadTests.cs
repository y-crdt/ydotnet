using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;
using YDotNet.Document.Options;

namespace YDotNet.Tests.Unit.Document;

public class LoadTests
{
    [Test]
    public void Load()
    {
        // Arrange
        var doc = new Doc();
        var map = doc.Map("sub-docs");
        var subDoc = new Doc(
            new DocOptions
            {
                ShouldLoad = false
            });

        // Assert
        Assert.That(subDoc.ShouldLoad, Is.False);

        // Act
        var transaction = doc.WriteTransaction();
        map.Insert(transaction, "sub-doc", Input.Doc(subDoc));
        transaction.Commit();

        // Assert
        Assert.That(subDoc.ShouldLoad, Is.False);

        // Act
        transaction = doc.WriteTransaction();
        subDoc.Load(transaction);
        transaction.Commit();

        // Assert
        Assert.That(doc.ShouldLoad, Is.True);
    }
}
