using System.Runtime.InteropServices;
using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Options;

namespace YDotNet.Tests.Unit.Document;

public class DocTests
{
    [Test]
    public void Create()
    {
        // Arrange and Act
        var doc = new Doc();

        // Assert
        Assert.That(doc.Handle, Is.GreaterThan(nint.Zero));
    }

    [Test]
    public void CreateWithSampleOptions()
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
    public void Dispose()
    {
        // Arrange
        var doc = new Doc();

        // Act
        doc.Dispose();

        // Assert
        Assert.That(doc.Handle, Is.EqualTo(nint.Zero));
    }

    [Test]
    public void GetIdOnActive()
    {
        // Arrange
        var doc = new Doc();

        // Act
        var id = doc.Id;

        // Assert
        Assert.That(id, Is.GreaterThan(expected: 0));
    }

    [Test]
    public void GetIdOnDisposed()
    {
        // Arrange
        var doc = new Doc();
        doc.Dispose();

        // Act and Assert
        Assert.Throws<SEHException>(
            () =>
            {
                var _ = doc.Id;
            });
    }
}
