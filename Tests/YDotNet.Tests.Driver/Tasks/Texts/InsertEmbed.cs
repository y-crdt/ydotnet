using YDotNet.Document;
using YDotNet.Document.Cells;
using YDotNet.Tests.Driver.Abstractions;

namespace YDotNet.Tests.Driver.Tasks.Texts;

public class InsertEmbed : ITask
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
                Console.WriteLine($"\tTexts:\t{count}");
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

                var transaction = doc.WriteTransaction();

                var inputs = new Dictionary<string, Input>
                {
                    { "null", Input.Null() },
                    { "false", Input.Boolean(value: false) },
                    { "true", Input.Boolean(value: true) },
                    { "bytes", Input.Bytes(new byte[] { 24, 69 }) },
                    { "double", Input.Double(value: 4.20) },
                    { "long", Input.Long(value: 1337L) },
                    { "string", Input.String("Lucas") }
                };
                var input = Input.Object(inputs);
                text.InsertEmbed(transaction, index: 0, input);

                transaction.Commit();

                foreach (var value in inputs.Values)
                {
                    value.Dispose();
                }

                input.Dispose();
                doc.Dispose();

                count++;
            }
        }

        return Task.CompletedTask;
    }
}
