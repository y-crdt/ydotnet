using YDotNet.Document;
using YDotNet.Tests.Driver.Abstractions;

namespace YDotNet.Tests.Driver.Tasks.Docs;

public class ObserveUpdatesV1 : ITask
{
    public Task Run()
    {
        var count = 0;

        // Create many documents
        while (count < 1_000_000)
        {
            // After 1s, stop and show the user the amount of documents
            if (count > 0)
            {
                Console.WriteLine("Status Report");
                Console.WriteLine($"\tDocs:\t{count}");
                Console.WriteLine();
            }

            if (count % 1_000 == 0)
            {
                Thread.Sleep(millisecondsTimeout: 15);
            }

            // Create many documents
            for (var i = 0; i < 100; i++)
            {
                var doc = new Doc();
                var text = doc.Text($"sample-{i}");

                doc.ObserveUpdatesV1(_ => { });

                var transaction = doc.WriteTransaction();
                text.Insert(transaction, index: 0, "YDotNet");
                transaction.Commit();

                doc.Dispose();

                count++;
            }
        }

        return Task.CompletedTask;
    }
}
