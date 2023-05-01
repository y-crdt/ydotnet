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
            Guid = "6811c0f5-320a-4a59-805d-ebe857a8b3f4",
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
        Assert.That(doc.Guid, Is.EqualTo("6811c0f5-320a-4a59-805d-ebe857a8b3f4"));
        Assert.That(doc.CollectionId, Is.EqualTo("sample_collection"));
        Assert.That(doc.AutoLoad, Is.EqualTo(expected: false));
        Assert.That(doc.ShouldLoad, Is.EqualTo(expected: false));

        // The properties `Encoding` and `SkipGarbageCollection` aren't accessible after construction.
    }

    [Test]
    public void DefaultNullOptions()
    {
        // Assert
        Assert.Throws<ArgumentNullException>(
            () =>
            {
                // Arrange and Act
                var _ = new Doc(options: null);
            });
    }

    [Test]
    public void ZeroId()
    {
        // Arrange
        var options = new DocOptions
        {
            Id = 0
        };

        // Act
        var doc = new Doc(options);

        // Assert
        Assert.That(doc.Id, Is.EqualTo(expected: 0));
    }

    [Test]
    public void EmptyCollectionId()
    {
        // Arrange
        var options = new DocOptions
        {
            CollectionId = string.Empty
        };

        // Act
        var doc = new Doc(options);

        // Assert
        Assert.That(doc.CollectionId, Is.EqualTo(string.Empty));
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
