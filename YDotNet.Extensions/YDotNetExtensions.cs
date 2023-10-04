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

        return ConvertValue(parsed);

        static Input ConvertObject(JsonElement element)
        {
            return Input.Object(element.EnumerateObject().ToDictionary(x => x.Name, x => ConvertValue(x.Value)));
        }

        static Input ConvertArray(JsonElement element)
        {
            return Input.Array(element.EnumerateArray().Select(ConvertValue).ToArray());
        }

        static Input ConvertValue(JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    return ConvertObject(element);
                case JsonValueKind.Array:
                    return ConvertArray(element);
                case JsonValueKind.String:
                    return Input.String(element.GetString() ?? string.Empty);
                case JsonValueKind.Number:
                    return Input.Double(element.GetDouble());
                case JsonValueKind.True:
                    return Input.Boolean(true);
                case JsonValueKind.False:
                    return Input.Boolean(false);
                case JsonValueKind.Null:
                    return Input.Null();
                default:
                    throw new NotSupportedException();
            }
        }
    }

    public static T? To<T>(this Output output, Doc doc)
    {
        using var transaction = doc.ReadTransaction()
            ?? throw new InvalidOperationException("Failed to open transaction.");

        return output.To<T>(transaction);
    }

    public static T? To<T>(this Output output, Transaction transaction)
    {
        var jsonStream = new MemoryStream();
        var jsonWriter = new Utf8JsonWriter(jsonStream);

        WriteValue(output, jsonWriter, transaction);

        static void WriteObject(IDictionary<string, Output> obj, Utf8JsonWriter jsonWriter, Transaction transaction)
        {
            jsonWriter.WriteStartObject();

            foreach (var (key, value) in obj)
            {
                WriteProperty(key, value, jsonWriter, transaction);
            }

            jsonWriter.WriteEndObject();
        }

        static void WriteCollection(Output[] array, Utf8JsonWriter jsonWriter, Transaction transaction)
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
            jsonWriter.WriteStartArray();

            foreach (var property in map.Iterate(transaction) ?? throw new InvalidOperationException("Failed to iterate array."))
            {
                WriteProperty(property.Key, property.Value, jsonWriter, transaction);
            }

            jsonWriter.WriteEndArray();
        }

        static void WriteArray(Array array, Utf8JsonWriter jsonWriter, Transaction transaction)
        {
            jsonWriter.WriteStartArray();

            foreach (var item in array.Iterate(transaction) ?? throw new InvalidOperationException("Failed to iterate array."))
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
            else if (output.Object is IDictionary<string, Output> obj)
            {
                WriteObject(obj, jsonWriter, transaction);
            }
            else if (output.Collection is Output[] collection)
            {
                WriteCollection(collection, jsonWriter, transaction);
            }
            else if (output.Map is Map map)
            {
                WriteMap(map, jsonWriter, transaction);
            }
            else if (output.Array is Array array)
            {
                WriteArray(array, jsonWriter, transaction);
            }
            else
            {
                throw new InvalidOperationException("Unsupported data type.");
            }
        }

        jsonWriter.Flush();
        jsonStream.Position = 0;

        return JsonSerializer.Deserialize<T>(jsonStream);
    }
}
