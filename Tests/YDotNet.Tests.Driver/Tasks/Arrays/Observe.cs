using YDotNet.Document;
using YDotNet.Document.Cells;
using YDotNet.Tests.Driver.Abstractions;

namespace YDotNet.Tests.Driver.Tasks.Arrays;

public class Observe : ITask
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
                Console.WriteLine($"\tArrays:\t{count}");
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
                var array = doc.Array($"sample-{i}");

                var subscription = array.Observe(_ => { });

                var transaction = doc.WriteTransaction();
                array.InsertRange(transaction, index: 0, Input.Long(value: 2469L));
                transaction.Commit();

                subscription.Dispose();
                doc.Dispose();
                count++;
            }
        }

        return Task.CompletedTask;
    }
}
