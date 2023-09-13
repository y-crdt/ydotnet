using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.StickyIndexes;

namespace YDotNet.Tests.Unit.StickyIndexes;

public class AssociationTypeTests
{
    [Test]
    public void ReturnsCorrectlyWithBefore()
    {
        // Arrange
        var doc = new Doc();
        var text = doc.Text("text");

        var transaction = doc.WriteTransaction();
        text.Insert(transaction, 0, "Lucas");
        var stickyIndex = text.StickyIndex(transaction, 3, StickyAssociationType.Before);
        transaction.Commit();

        // Act
        var associationType = stickyIndex.AssociationType;

        // Assert
        Assert.That(associationType, Is.EqualTo(StickyAssociationType.Before));
    }

    [Test]
    public void ReturnsCorrectlyWithAfter()
    {
        // Arrange
        var doc = new Doc();
        var text = doc.Text("text");

        var transaction = doc.WriteTransaction();
        text.Insert(transaction, 0, "Lucas");
        var stickyIndex = text.StickyIndex(transaction, 3, StickyAssociationType.After);
        transaction.Commit();

        // Act
        var associationType = stickyIndex.AssociationType;

        // Assert
        Assert.That(associationType, Is.EqualTo(StickyAssociationType.After));
    }
}
