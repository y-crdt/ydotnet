using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Options;
using YDotNet.Document.StickyIndexes;

namespace YDotNet.Tests.Unit.StickyIndexes;

public class EncodeTests
{
    [Test]
    public void EncodesOnEmptyValue()
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
        var result = stickyIndex.Encode();

        // Assert
        Assert.That(result, Is.EqualTo(new byte[] { 1, 4, 116, 101, 120, 116, 65 }));
    }

    [Test]
    public void EncodesOnFilledValue()
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
        var result = stickyIndex.Encode();

        // Assert
        Assert.That(result, Is.EqualTo(new byte[] { 0, 73, 2, 65 }));
    }
}
