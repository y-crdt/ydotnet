using System.Text.Json;
using YDotNet.Document.Cells;

namespace YDotNet.Extensions;

public static class InputFactory
{
    public static Input FromJson(string json)
    {
        using (JsonDocument doc = JsonDocument.Parse(json))
        {
            return FromJson(doc.RootElement);
        }
    }

    public static Input FromJson(JsonElement json)
    {
        return ConvertValue(json);

        static Input ConvertObject(JsonElement element)
        {
            return Input.Object(element.EnumerateObject().ToDictionary(x => x.Name, x => ConvertValue(x.Value), StringComparer.Ordinal));
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
}
