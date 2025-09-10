using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;
using YDotNet.Document.Types.XmlTexts;

namespace YDotNet.Tests.Unit.XmlTexts;

public class ChunksTests
{
    [Test]
    public void ChunkIsTheWholeText()
    {
        // Arrange
        var xmlText = ArrangeXmlText(0, (uint)"Lucas".Length);

        // Act
        var transaction = xmlText.ReadTransaction();
        var diff = xmlText.Chunks(transaction);
        transaction.Dispose();

        // Assert
        Assert.That(diff, Has.Count.EqualTo(1));
    }

    [Test]
    public void ChunksFormattedAtBeginning()
    {
        // Arrange
        var xmlText = ArrangeXmlText(index: 0, length: 2);

        // Act
        var transaction = xmlText.ReadTransaction();
        var chunks = xmlText.Chunks(transaction);
        transaction.Commit();

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
    public void ChunksFormattedAtEnding()
    {
        // Arrange
        var xmlText = ArrangeXmlText(index: 3, length: 2);

        // Act
        var transaction = xmlText.ReadTransaction();
        var chunks = xmlText.Chunks(transaction);
        transaction.Commit();

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

    [Test]
    public void ChunksFormattedAtMiddle()
    {
        // Arrange
        var xmlText = ArrangeXmlText(index: 2, length: 2);

        // Act
        var transaction = xmlText.ReadTransaction();
        var chunks = xmlText.Chunks(transaction);
        transaction.Commit();

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

    private XmlText ArrangeXmlText(uint index, uint length)
    {
        var doc = new Doc();
        var xmlFragment = doc.XmlFragment("xml-fragment");

        var transaction = doc.WriteTransaction();
        var xmlText = xmlFragment.InsertText(transaction, 0);
        xmlText.Insert(transaction, index: 0, "Lucas");
        xmlText.Format(
            transaction, index, length, Input.Object(
                new Dictionary<string, Input>
                {
                    { "bold", Input.Boolean(value: true) }
                }));

        transaction.Commit();

        return xmlText;
    }
}
