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
            Encoding = DocEncoding.Utf16,
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
    public void AutoLoadEnabled()
    {
        // Arrange
        var options = new DocOptions
        {
            AutoLoad = true
        };

        // Act
        var doc = new Doc(options);

        // Assert
        Assert.That(doc.AutoLoad, Is.True);
    }

    [Test]
    public void AutoLoadDisabled()
    {
        // Arrange
        var options = new DocOptions
        {
            AutoLoad = false
        };

        // Act
        var doc = new Doc(options);

        // Assert
        Assert.That(doc.AutoLoad, Is.False);
    }

    [Test]
    public void ShouldLoadEnabled()
    {
        // Arrange
        var options = new DocOptions
        {
            ShouldLoad = false
        };

        // Act
        var doc = new Doc(options);

        // Assert
        Assert.That(doc.ShouldLoad, Is.False);
    }

    [Test]
    public void ShouldLoadDisabled()
    {
        // Arrange
        var options = new DocOptions
        {
            ShouldLoad = false
        };

        // Act
        var doc = new Doc(options);

        // Assert
        Assert.That(doc.ShouldLoad, Is.False);
    }
}
