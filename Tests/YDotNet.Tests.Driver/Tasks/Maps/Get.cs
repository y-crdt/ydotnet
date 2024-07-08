using YDotNet.Document;
using YDotNet.Document.Cells;
using YDotNet.Tests.Driver.Abstractions;

namespace YDotNet.Tests.Driver.Tasks.Maps;

public class Get : ITask
{
    public Task Run()
    {
        var count = 0;
        var random = new Random(0);

        // Create many documents
        while (count < 1_000_000)
        {
            // After 1s, stop and show the user the amount of documents
            if (count > 0)
            {
                Console.WriteLine("Status Report");
                Console.WriteLine($"\tMaps:\t{count}");
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
                var map = doc.Map($"sample-{i}");

                var transaction = doc.WriteTransaction();
                var value = random.Next(minValue: 1, maxValue: 15);

                // TODO [LSViana] Check reading `Doc` instances.
                var input = value switch
                {
                    1 => Input.String("value"),
                    2 => Input.Boolean(value: false),
                    3 => Input.Double(24.69),
                    4 => Input.Long(value: 2469L),
                    5 => Input.Bytes([2, 4, 6, 9]),
                    6 => Input.Collection([Input.Long(value: 6942L)]),
                    7 => Input.Object(new Dictionary<string, Input>
                    {
                        { "value", Input.Double(13.37) }
                    }),
                    8 => Input.Null(),
                    9 => Input.Undefined(),
                    10 => Input.Array([Input.Long(value: 420L)]),
                    11 => Input.Map(new Dictionary<string, Input>
                    {
                        { "value", Input.Long(2469L) }
                    }),
                    12 => Input.Text("Y-CRDT"),
                    13 => Input.XmlElement("html"),
                    14 => Input.XmlText("YDotNet"),
                    _ => throw new InvalidOperationException()
                };

                map.Insert(transaction, "value", input);
                var output = map.Get(transaction, "value");
                transaction.Commit();

                object _ = value switch
                {
                    1 => output.String,
                    2 => output.Boolean,
                    3 => output.Double,
                    4 => output.Long,
                    5 => output.Bytes,
                    6 => output.JsonArray,
                    7 => output.JsonObject,
                    8 => output.Null,
                    9 => output.Undefined,
                    10 => output.Array,
                    11 => output.Map,
                    12 => output.Text,
                    13 => output.XmlElement,
                    14 => output.XmlText,
                    _ => throw new InvalidOperationException()
                };

                doc.Dispose();

                count++;
            }
        }

        return Task.CompletedTask;
    }
}
