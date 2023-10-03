using System.Text.Json;
using YDotNet.Document;
using YDotNet.Document.Cells;
using YDotNet.Document.Transactions;
using YDotNet.Document.Types.Maps;
using Array = YDotNet.Document.Types.Arrays.Array;

namespace YDotNet.Extensions;

public static class YDotNetExtensions
{
    public static T? To<T>(this Output output, Doc doc)
    {
        var jsonStream = new MemoryStream();
        var jsonWriter = new Utf8JsonWriter(jsonStream);

        using var transaction = doc.ReadTransaction()
            ?? throw new InvalidOperationException("Failed to open transaction.");

        WriteValue(output, jsonWriter, transaction);

        static void WriteMap(Map map, Utf8JsonWriter jsonWriter, Transaction transaction)
        {
            var properties = map.Iterate(transaction)
                ?? throw new InvalidOperationException("Failed to iterate object.");

            foreach (var property in properties)
            {
                WriteProperty(property, jsonWriter, transaction);
            }
        }

        static void WriteArray(Array array, Utf8JsonWriter jsonWriter, Transaction transaction)
        {
            var items = array.Iterate(transaction)
                ?? throw new InvalidOperationException("Failed to iterate array.");

            foreach (var item in items)
            {
                WriteValue(item, jsonWriter, transaction);
            }
        }

        static void WriteProperty(MapEntry property, Utf8JsonWriter jsonWriter, Transaction transaction)
        {
            jsonWriter.WritePropertyName(property.Key);

            WriteValue(property.Value, jsonWriter, transaction);
        }

        static void WriteValue(Output output, Utf8JsonWriter jsonWriter, Transaction transaction)
        {
            if (output.String != null)
            {
                jsonWriter.WriteStringValue(output.String);
            }
            else if (output.Boolean != null)
            {
                jsonWriter.WriteBooleanValue(output.Boolean.Value);
            }
            else if (output.Double != null)
            {
                jsonWriter.WriteNumberValue(output.Double.Value);
            }
            else if (output.Null)
            {
                jsonWriter.WriteNullValue();
            }
            else if (output.Map is Map map)
            {
                WriteMap(map, jsonWriter, transaction);
            }
            else if (output.Array is Array array)
            {
                WriteArray(array, jsonWriter, transaction);
            }

            throw new InvalidOperationException("Unsupported data type.");
        }

        jsonWriter.Flush();
        jsonStream.Position = 0;

        return JsonSerializer.Deserialize<T>(jsonStream);
    }
}
