using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Transactions;
using YDotNet.Document.Types.Texts;

namespace YDotNet.Tests.Unit.Texts;

public class RemoveRangeTests
{
    [Test]
    public void RemoveAtBeginning()
    {
        // Arrange
        var (text, transaction) = ArrangeText();

        // Act
        text.RemoveRange(transaction, index: 0, length: 2);

        // Assert
        Assert.That(text.String(transaction), Is.EqualTo("cas"));
    }

    [Test]
    public void RemoveAtMiddle()
    {
        // Arrange
        var (text, transaction) = ArrangeText();

        // Act
        text.RemoveRange(transaction, index: 2, length: 2);

        // Assert
        Assert.That(text.String(transaction), Is.EqualTo("Lus"));
    }

    [Test]
    public void RemoveAtEnding()
    {
        // Arrange
        var (text, transaction) = ArrangeText();

        // Act
        text.RemoveRange(transaction, index: 3, length: 2);

        // Assert
        Assert.That(text.String(transaction), Is.EqualTo("Luc"));
    }

    [Test]
    public void RemoveFullText()
    {
        // Arrange
        var (text, transaction) = ArrangeText();

        // Act
        text.RemoveRange(transaction, index: 0, length: 5);

        // Assert
        Assert.That(text.String(transaction), Is.EqualTo(string.Empty));
    }

    [Test]
    public void RemoveNothing()
    {
        // Arrange
        var (text, transaction) = ArrangeText();

        // Act
        text.RemoveRange(transaction, index: 0, length: 0);

        // Assert
        Assert.That(text.String(transaction), Is.EqualTo("Lucas"));
    }

    private (Text, Transaction) ArrangeText()
    {
        var doc = new Doc();
        var text = doc.Text("value");
        var transaction = doc.WriteTransaction();

        text.Insert(transaction, index: 0, "Lucas");

        return (text, transaction);
    }
}
