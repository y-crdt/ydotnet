using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Options;

namespace YDotNet.Tests.Unit.Document;

public class NewWithOptionsTests
{
    [Test]
    public void SampleOptions()
    {
        // Arrange
        var options = new DocOptions
        {
            Id = 2718,
            Guid = Guid.Parse("6811c0f5-320a-4a59-805d-ebe857a8b3f4"),
            CollectionId = "sample_collection",
            Encoding = DocEncoding.Uf16,
            SkipGarbageCollection = false,
            AutoLoad = false,
            ShouldLoad = false
        };

        // Act
        var doc = new Doc(options);

        // Assert
        Assert.That(doc.Handle, Is.GreaterThan(nint.Zero));
        Assert.That(doc.Id, Is.EqualTo(expected: 2718));
        // TODO Add assertion for Guid.
        // TODO Add assertion for CollectionId.
        // TODO Add assertion for Encoding.
        // TODO Add assertion for SkipGarbageCollection.
        // TODO Add assertion for AutoLoad.
        // TODO Add assertion for ShouldLoad.
    }

    [Test]
    [Ignore("Not implemented yet.")]
    public void DefaultNullOptions()
    {
    }

    [Test]
    [Ignore("Not implemented yet.")]
    public void NegativeId()
    {
    }

    [Test]
    [Ignore("Not implemented yet.")]
    public void ZeroId()
    {
    }

    [Test]
    [Ignore("Not implemented yet.")]
    public void EmptyCollectionId()
    {
    }

    [Test]
    [Ignore("Not implemented yet.")]
    public void EncodingUtf8()
    {
    }

    [Test]
    [Ignore("Not implemented yet.")]
    public void EncodingUtf16()
    {
    }

    [Test]
    [Ignore("Not implemented yet.")]
    public void EncodingUtf32()
    {
    }

    [Test]
    [Ignore("Not implemented yet.")]
    public void SkipGarbageCollectionEnabled()
    {
    }

    [Test]
    [Ignore("Not implemented yet.")]
    public void SkipGarbageCollectionDisabled()
    {
    }

    [Test]
    [Ignore("Not implemented yet.")]
    public void AutoLoadEnabled()
    {
    }

    [Test]
    [Ignore("Not implemented yet.")]
    public void AutoLoadDisabled()
    {
    }

    [Test]
    [Ignore("Not implemented yet.")]
    public void ShouldLoadEnabled()
    {
    }

    [Test]
    [Ignore("Not implemented yet.")]
    public void ShouldLoadDisabled()
    {
    }
}
