using YDotNet.Document;
using YDotNet.Tests.Driver.Abstractions;

namespace YDotNet.Tests.Driver.Tasks.Docs;

public class ReadId : ITask
{
    public Task Run()
    {
        var count = 0;
        var doc = new Doc();

        // Read many times
        while (count < 1_000_000)
        {
            // After 1s, stop and show the user the amount of documents
            if (count > 0)
            {
                Console.WriteLine("Status Report");
                Console.WriteLine($"\tReads:\t{count}");
                Console.WriteLine();
            }

            if (count % 1_000 == 0)
            {
                Thread.Sleep(millisecondsTimeout: 15);
            }

            // Create many documents
            for (var i = 0; i < 100; i++)
            {
                var _ = doc.Id;
                count++;
            }
        }

        doc.Dispose();

        return Task.CompletedTask;
    }
}
