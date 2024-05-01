using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;
using YDotNet.Document.Transactions;
using YDotNet.Document.Types.Texts;

namespace YDotNet.Tests.Unit.Texts;

public class ChunksTests
{
    [Test]
    public void ChunkIsTheWholeText()
    {
        // Arrange
        var doc = new Doc();
        var text = doc.Text("value");

        var transaction = doc.WriteTransaction();
        text.Insert(transaction, index: 0, "Lucas");

        // Act
        var chunks = text.Chunks(transaction);

        // Assert
        Assert.That(chunks.Count, Is.EqualTo(expected: 1));
        Assert.That(chunks.First().Attributes, Is.Empty);

        transaction.Commit();
    }

    [Test]
    public void ChunksFormattedAtBeginning()
    {
        // Arrange
        var (text, transaction) = ArrangeText(index: 0, length: 2);

        // Act
        var chunks = text.Chunks(transaction);

        // Assert
        Assert.That(chunks.Count, Is.EqualTo(expected: 2));

        var firstChunk = chunks.ElementAt(index: 0);
        var firstChunkAttribute = firstChunk.Attributes.First();

        Assert.That(firstChunk.Data.String, Is.EqualTo("Lu"));
        Assert.That(firstChunk.Attributes.Count(), Is.EqualTo(expected: 1));
        Assert.That(firstChunkAttribute.Key, Is.EqualTo("bold"));
        Assert.That(firstChunkAttribute.Value.Boolean, Is.True);

        var secondChunk = chunks.ElementAt(index: 1);

        Assert.That(secondChunk.Data.String, Is.EqualTo("cas"));
        Assert.That(secondChunk.Attributes.Count(), Is.EqualTo(expected: 0));
    }

    [Test]
    public void ChunksFormattedAtMiddle()
    {
        // Arrange
        var (text, transaction) = ArrangeText(index: 2, length: 2);

        // Act
        var chunks = text.Chunks(transaction);

        // Assert
        Assert.That(chunks.Count, Is.EqualTo(expected: 3));

        var firstChunk = chunks.ElementAt(index: 0);

        Assert.That(firstChunk.Data.String, Is.EqualTo("Lu"));
        Assert.That(firstChunk.Attributes.Count(), Is.EqualTo(expected: 0));

        var secondChunk = chunks.ElementAt(index: 1);
        var secondChunkAttribute = secondChunk.Attributes.First();

        Assert.That(secondChunk.Data.String, Is.EqualTo("ca"));
        Assert.That(secondChunk.Attributes.Count(), Is.EqualTo(expected: 1));
        Assert.That(secondChunkAttribute.Key, Is.EqualTo("bold"));
        Assert.That(secondChunkAttribute.Value.Boolean, Is.True);

        var thirdChunk = chunks.ElementAt(index: 2);

        Assert.That(thirdChunk.Data.String, Is.EqualTo("s"));
        Assert.That(thirdChunk.Attributes.Count(), Is.EqualTo(expected: 0));
    }

    [Test]
    public void ChunksFormattedAtEnding()
    {
        // Arrange
        var (text, transaction) = ArrangeText(index: 3, length: 2);

        // Act
        var chunks = text.Chunks(transaction);

        // Assert
        Assert.That(chunks.Count, Is.EqualTo(expected: 2));

        var firstChunk = chunks.ElementAt(index: 0);

        Assert.That(firstChunk.Data.String, Is.EqualTo("Luc"));
        Assert.That(firstChunk.Attributes.Count(), Is.EqualTo(expected: 0));

        var secondChunk = chunks.ElementAt(index: 1);
        var secondChunkAttribute = secondChunk.Attributes.First();

        Assert.That(secondChunk.Data.String, Is.EqualTo("as"));
        Assert.That(secondChunk.Attributes.Count(), Is.EqualTo(expected: 1));
        Assert.That(secondChunkAttribute.Key, Is.EqualTo("bold"));
        Assert.That(secondChunkAttribute.Value.Boolean, Is.True);
    }

    private (Text, Transaction) ArrangeText(uint index, uint length)
    {
        var doc = new Doc();
        var text = doc.Text("value");

        var transaction = doc.WriteTransaction();

        text.Insert(transaction, index: 0, "Lucas");
        text.Format(
            transaction, index, length, Input.Object(
                new Dictionary<string, Input>
                {
                    { "bold", Input.Boolean(value: true) }
                }));

        transaction.Commit();

        return (text, transaction);
    }
}
