using YDotNet.Document;
using YDotNet.Document.Options;
using YDotNet.Tests.Driver.Abstractions;

namespace YDotNet.Tests.Driver.Tasks.Docs;

public class CreateWithOptions : ITask
{
    public Task Run()
    {
        var count = 0;

        // Create many documents
        while (count < 2_000_000)
        {
            // After 1s, stop and show the user the amount of documents
            if (count > 0)
            {
                Console.WriteLine("Status Report");
                Console.WriteLine($"\tDocuments:\t{count}");
                Console.WriteLine();
            }

            if (count % 1_000 == 0)
            {
                Thread.Sleep(millisecondsTimeout: 20);
            }

            // Create many documents
            for (var i = 0; i < 100; i++)
            {
                var options = new DocOptions
                {
                    Encoding = DocEncoding.Utf8,
                    Guid = Guid.NewGuid().ToString(),
                    Id = (ulong) Random.Shared.NextInt64(),
                    AutoLoad = false,
                    CollectionId = "random-collection",
                    ShouldLoad = false,
                    SkipGarbageCollection = true
                };

                new Doc(options).Dispose();
                count++;
            }
        }

        return Task.CompletedTask;
    }
}
