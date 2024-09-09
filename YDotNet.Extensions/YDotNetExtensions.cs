using System.Text;
using System.Text.Json;
using YDotNet.Document;
using YDotNet.Document.Cells;
using YDotNet.Document.Transactions;
using YDotNet.Document.Types.Maps;
using Array = YDotNet.Document.Types.Arrays.Array;

namespace YDotNet.Extensions;

public static class YDotNetExtensions
{
    public static Input ToInput<T>(this T source)
    {
        var parsed = JsonSerializer.SerializeToElement(source);

        return InputFactory.FromJson(parsed);
    }

    public static T To<T>(this Output output, Doc doc)
    {
        using var transaction = doc.ReadTransaction();

        return output.To<T>(transaction);
    }

    public static T To<T>(this Output output, Transaction transaction)
    {
        var jsonStream = new MemoryStream();

        output.ToJson(jsonStream, transaction);

        jsonStream.Flush();
        jsonStream.Position = 0;

        return JsonSerializer.Deserialize<T>(jsonStream)!;
    }

    public static string ToJson(this Output output, Doc doc)
    {
        using var transaction = doc.ReadTransaction();

        return output.ToJson(transaction);
    }

    public static string ToJson(this Output output, Transaction transaction)
    {
        var jsonStream = new MemoryStream();

        output.ToJson(jsonStream, transaction);

        jsonStream.Flush();
        jsonStream.Position = 0;

        return Encoding.UTF8.GetString(jsonStream.ToArray());
    }

#pragma warning disable MA0051 // Method is too long
    public static void ToJson(this Output output, Stream stream, Transaction transaction)
#pragma warning restore MA0051 // Method is too long
    {
        var jsonWriter = new Utf8JsonWriter(stream);

        WriteValue(output, jsonWriter, transaction);

        jsonWriter.Flush();

        static void WriteJsonObject(JsonObject obj, Utf8JsonWriter jsonWriter, Transaction transaction)
        {
            jsonWriter.WriteStartObject();

            foreach (var (key, value) in obj)
            {
                WriteProperty(key, value, jsonWriter, transaction);
            }

            jsonWriter.WriteEndObject();
        }

        static void WriteCollection(JsonArray array, Utf8JsonWriter jsonWriter, Transaction transaction)
        {
            jsonWriter.WriteStartArray();

            foreach (var item in array)
            {
                WriteValue(item, jsonWriter, transaction);
            }

            jsonWriter.WriteEndArray();
        }

        static void WriteMap(Map map, Utf8JsonWriter jsonWriter, Transaction transaction)
        {
            jsonWriter.WriteStartObject();

            foreach (var property in map.Iterate(transaction))
            {
                WriteProperty(property.Key, property.Value, jsonWriter, transaction);
            }

            jsonWriter.WriteEndObject();
        }

        static void WriteArray(Array array, Utf8JsonWriter jsonWriter, Transaction transaction)
        {
            jsonWriter.WriteStartArray();

            foreach (var item in array.Iterate(transaction))
            {
                WriteValue(item, jsonWriter, transaction);
            }

            jsonWriter.WriteEndArray();
        }

        static void WriteProperty(string key, Output value, Utf8JsonWriter jsonWriter, Transaction transaction)
        {
            jsonWriter.WritePropertyName(key);

            WriteValue(value, jsonWriter, transaction);
        }

        static void WriteValue(Output output, Utf8JsonWriter jsonWriter, Transaction transaction)
        {
            switch (output.Tag)
            {
                case OutputTag.Boolean:
                    jsonWriter.WriteBooleanValue(output.Boolean);
                    break;
                case OutputTag.Double:
                    jsonWriter.WriteNumberValue(output.Double);
                    break;
                case OutputTag.Long:
                    jsonWriter.WriteNumberValue(output.Long);
                    break;
                case OutputTag.String:
                    jsonWriter.WriteStringValue(output.String);
                    break;
                case OutputTag.Text:
                    jsonWriter.WriteStringValue(output.Text.String(transaction));
                    break;
                case OutputTag.JsonArray:
                    WriteCollection(output.JsonArray, jsonWriter, transaction);
                    break;
                case OutputTag.JsonObject:
                    WriteJsonObject(output.JsonObject, jsonWriter, transaction);
                    break;
                case OutputTag.Null:
                    jsonWriter.WriteNullValue();
                    break;
                case OutputTag.Undefined:
                    jsonWriter.WriteNullValue();
                    break;
                case OutputTag.Array:
                    WriteArray(output.Array, jsonWriter, transaction);
                    break;
                case OutputTag.Map:
                    WriteMap(output.Map, jsonWriter, transaction);
                    break;
                case OutputTag.Doc:
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported data type \"{output.Tag}\".");
            }
        }
    }
}
