using NUnit.Framework;
using System.Text.Json.Serialization;
using YDotNet.Document;
using YDotNet.Document.Cells;
using YDotNet.Extensions;

namespace YDotNet.Tests.Unit.Extensions;

internal class Conversion
{
    public class Expected
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }
    }

    [Test]
    public void ConvertTo()
    {
        // Arrange
        var doc = new Doc();
        var map = doc.Map("map");

        using (var transaction = map.WriteTransaction())
        {
            map.Insert(transaction, "inner", Input.Object(new Dictionary<string, Input>
            {
                ["value"] = Input.String("Hello YDotNet")
            }));
        }

        // Act
        Expected result;
        using (var transaction = map.WriteTransaction())
        {
            var inner = map.Get(transaction, "inner");

            result = inner.To<Expected>(transaction);
        }

        // Assert
        Assert.That(result.Value, Is.EqualTo("Hello YDotNet"));
    }

    [Test]
    public void ConvertToJson()
    {
        // Arrange
        var doc = new Doc();
        var map = doc.Map("map");

        using (var transaction = map.WriteTransaction())
        {
            map.Insert(transaction, "inner", Input.Object(new Dictionary<string, Input>
            {
                ["value"] = Input.String("Hello YDotNet")
            }));
        }

        // Act
        string json;
        using (var transaction = map.WriteTransaction())
        {
            var inner = map.Get(transaction, "inner");

            json = inner.ToJson(transaction);
        }

        // Assert
        StringAssert.Contains("Hello YDotNet", json);
    }
}
