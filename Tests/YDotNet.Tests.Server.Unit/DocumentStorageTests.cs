namespace YDotNet.Tests.Server.Unit;

using NUnit.Framework;
using YDotNet.Server.Storage;

public abstract class DocumentStorageTests
{
    protected abstract IDocumentStorage CreateSut();

    [Test]
    public async Task GetDocData()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        var received = await sut.GetDocAsync(Guid.NewGuid().ToString());

        // Assert
        Assert.That(received, Is.Null);
    }

    [Test]
    public async Task InsertDocument()
    {
        // Arrange
        var sut = CreateSut();

        var docName = Guid.NewGuid().ToString();
        var docData = new byte[] { 1, 2, 3, 4 };

        // Act
        await sut.StoreDocAsync(docName, docData);

        // Assert
        var received = await sut.GetDocAsync(docName);

        Assert.That(received, Is.EqualTo(docData));
    }

    [Test]
    public async Task UpdateDocument()
    {
        // Arrange
        var sut = CreateSut();

        var docName = Guid.NewGuid().ToString();
        var docData1 = new byte[] { 1, 2, 3, 4 };
        var docData2 = new byte[] { 5, 6, 7, 8 };

        // Act
        await sut.StoreDocAsync(docName, docData1);
        await sut.StoreDocAsync(docName, docData2);

        // Assert
        var received = await sut.GetDocAsync(docName);

        Assert.That(received, Is.EqualTo(docData2));
    }
}
