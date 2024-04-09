using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YDotNet.Document.Cells;
using YDotNet.Server;
using YDotNet.Server.Internal;
using YDotNet.Server.Storage;

namespace YDotNet.Tests.Server.Unit;

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

            await Task.Delay(100);
            return true;
        });

        A.CallTo(() => documentStorage.StoreDocAsync(name, A<byte[]>._, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    private DocumentContainer CreateSut(DocumentManagerOptions? options)
    {
        options ??= new DocumentManagerOptions();

        return new DocumentContainer(name, documentStorage, documentCallback, documentManager, options,
            A.Fake<ILogger>());
    }
}
