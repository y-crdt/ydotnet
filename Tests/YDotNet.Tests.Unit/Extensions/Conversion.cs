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
    public void CanParseMapsToObject()
    {
        // Arrange
        var doc = new Doc();
        var items = doc.Array("items");

        using (var transaction = doc.WriteTransaction())
        {
            var map = Input.Map(new Dictionary<string, Input>()
            {
                ["value"] = Input.String("Hello YDotNet"),
            });
            items.InsertRange(transaction, 0, map);
        }

        using (var transaction = doc.ReadTransaction())
        {
            var map = items.Get(transaction, 0);
            
            // Act
            var parsed = map.To<Expected>(transaction);
            
            // Assert
            Assert.That(parsed.Value, Is.EqualTo("Hello YDotNet"));
        }
    }

    [Test]
    public void CanParseTextToString()
    {
        // Arrange
        var doc = new Doc();
        var map = doc.Map("map");

        using (var transaction = doc.WriteTransaction())
        {
            map.Insert(transaction, "inner", Input.Map(new Dictionary<string, Input>
            {
                {"value", Input.Text("Hello YDotNet")}
            }));
        }

        using (var transaction = doc.ReadTransaction())
        {
            var inner = map.Get(transaction, "inner");
            
            // Act
            var parsed = inner.To<Expected>(transaction);

            // Assert
            Assert.That(parsed.Value, Is.EqualTo("Hello YDotNet"));
        }
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
        using (var transaction = map.ReadTransaction())
        {
            var inner = map.Get(transaction, "inner");

            json = inner.ToJson(transaction);
        }

        // Assert
        StringAssert.Contains("Hello YDotNet", json);
    }

    [Test]
    public void ConvertTextToJson()
    {
        // Arrange
        var doc = new Doc();
        var map = doc.Map("map");

        using (var transaction = doc.WriteTransaction())
        {
            map.Insert(transaction, "inner", Input.Map(new Dictionary<string, Input>
            {
                {"value", Input.Text("Hello YDotNet")}
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
