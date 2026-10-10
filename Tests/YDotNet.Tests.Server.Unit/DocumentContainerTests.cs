namespace YDotNet.Tests.Server.Unit;

using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
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
        var stored = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        A.CallTo(() => documentStorage.StoreDocAsync(name, A<byte[]>._, A<CancellationToken>._))
            .Invokes(() => stored.TrySetResult());

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

        // The write runs in the background and needs the lock that the action above held, so it can only
        // start once the action has returned. Wait for it instead of asserting immediately.
        await stored.Task.WaitAsync(TimeSpan.FromSeconds(10)).ConfigureAwait(false);

        A.CallTo(() => documentStorage.StoreDocAsync(name, A<byte[]>._, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
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
