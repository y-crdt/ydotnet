using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.Texts;

public class StringTests
{
    [Test]
    public void ReturnsTheFullText()
    {
        // Arrange
        var doc = new Doc();
        var text = doc.Text("name");

        // Act
        var transaction = doc.WriteTransaction();
        text.Insert(transaction, index: 0, "Star. ⭐");

        // Assert
        Assert.That(text.String(transaction), Is.EqualTo("Star. ⭐"));

        transaction.Commit();
    }
}
