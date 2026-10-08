namespace YDotNet.Tests.Server.Unit;

using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;
using YDotNet.Server;
using YDotNet.Server.Internal;
using YDotNet.Server.Storage;

public class DocumentContainerTests
{
    private readonly IDocumentStorage documentStorage = A.Fake<IDocumentStorage>();
    private readonly IDocumentCallback documentCallback = A.Fake<IDocumentCallback>();
    private readonly IDocumentManager documentManager = A.Fake<IDocumentManager>();
    private readonly string name = Guid.NewGuid().ToString();

    [Test]
    public async Task StoreImmediately()
    {
        var sut = CreateSut(new DocumentManagerOptions
        {
            StoreDebounce = TimeSpan.Zero,
        });

        await sut.ApplyUpdateReturnAsync(async doc =>
        {
            var map = doc.Map("map");
            using (var transaction = doc.WriteTransaction())
            {
                map.Insert(transaction, "key", Input.Double(42));
            }

            await Task.Delay(100).ConfigureAwait(false);
            return true;
        });

        A.CallTo(() => documentStorage.StoreDocAsync(name, A<byte[]>._, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Test]
    public async Task LoadsStoredDocument()
    {
        var source = new Doc();
        var sourceText = source.Text("text");

        using (var transaction = source.WriteTransaction())
        {
            sourceText.Insert(transaction, index: 0, "stored");
        }

        byte[] data;
        using (var transaction = source.ReadTransaction())
        {
            data = transaction.StateDiffV1(stateVector: null)!;
        }

        A.CallTo(() => documentStorage.GetDocAsync(name, A<CancellationToken>._))
            .Returns(data);

        var sut = CreateSut(options: null);

        var result = await sut.ApplyUpdateReturnAsync(doc =>
        {
            var text = doc.Text("text");

            using var transaction = doc.ReadTransaction();

            return Task.FromResult(text.String(transaction));
        });

        Assert.That(result, Is.EqualTo("stored"));
    }

    [Test]
    public async Task FailsWhenStoredDocumentCannotBeApplied()
    {
        A.CallTo(() => documentStorage.GetDocAsync(name, A<CancellationToken>._))
            .Returns(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });

        var sut = CreateSut(options: null);

        // Loading must fail instead of exposing an empty document that would replace the stored data.
        Assert.ThrowsAsync<YDotNetException>(() => sut.ApplyUpdateReturnAsync(_ => Task.FromResult(true)));

        await Task.Delay(100).ConfigureAwait(false);

        A.CallTo(() => documentStorage.StoreDocAsync(name, A<byte[]>._, A<CancellationToken>._))
            .MustNotHaveHappened();
    }

    private DocumentContainer CreateSut(DocumentManagerOptions? options)
    {
        options ??= new DocumentManagerOptions();

        return new DocumentContainer(
            name,
            documentStorage,
            documentCallback,
            documentManager,
            options,
            A.Fake<ILogger>());
    }
}
