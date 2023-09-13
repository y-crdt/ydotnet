using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Options;
using YDotNet.Document.StickyIndexes;

namespace YDotNet.Tests.Unit.StickyIndexes;

public class DecodeTests
{
    [Test]
    public void DecodesFromEmptyValue()
    {
        // Arrange
        var doc = new Doc(
            new DocOptions
            {
                Id = 91
            });
        var text = doc.Text("text");

        var transaction = doc.WriteTransaction();
        var stickyIndex = text.StickyIndex(transaction, index: 0, StickyAssociationType.Before);
        transaction.Commit();

        // Act
        transaction = doc.WriteTransaction();
        var decodedStickyIndex = StickyIndex.Decode(stickyIndex.Encode());
        var index = decodedStickyIndex?.Read(transaction);
        transaction.Commit();

        // Assert
        Assert.That(decodedStickyIndex, Is.Not.Null);
        Assert.That(index, Is.EqualTo(expected: 0));
    }

    [Test]
    public void DecodesOnFilledValue()
    {
        // Arrange
        var doc = new Doc(
            new DocOptions
            {
                Id = 73
            });
        var text = doc.Text("text");

        var transaction = doc.WriteTransaction();
        text.Insert(transaction, index: 0, "Lucas");
        var stickyIndex = text.StickyIndex(transaction, index: 3, StickyAssociationType.Before);
        transaction.Commit();

        // Act
        transaction = doc.WriteTransaction();
        var decodedStickyIndex = StickyIndex.Decode(stickyIndex.Encode());
        var index = decodedStickyIndex?.Read(transaction);
        transaction.Commit();

        // Assert
        Assert.That(decodedStickyIndex, Is.Not.Null);
        Assert.That(index, Is.EqualTo(expected: 3));
    }
}
