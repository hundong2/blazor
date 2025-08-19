using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace DataInspector.SharedComponents.Models.JsonBuilder
{
    public enum JsonNodeType
    {
        Object,
        Array,
        Value
    }

    public abstract class JsonNode
    {
        public JsonNode? Parent { get; set; }
        public abstract JsonNodeType Type { get; }
        public abstract JsonNode Clone();
        public abstract JsonNode ToJsonNode(); // To be used with System.Text.Json.Nodes
    }

    public class JsonObjectNode : JsonNode
    {
        public override JsonNodeType Type => JsonNodeType.Object;
        public List<KeyValuePair<string, JsonNode>> Properties { get; set; } = new List<KeyValuePair<string, JsonNode>>();

        public override JsonNode Clone()
        {
            var clone = new JsonObjectNode { Parent = this.Parent };
            foreach (var prop in Properties)
            {
                clone.Properties.Add(new KeyValuePair<string, JsonNode>(prop.Key, prop.Value.Clone()));
            }
            return clone;
        }

        public override JsonNode ToJsonNode()
        {
            var jObject = new JsonObject();
            foreach (var prop in Properties)
            {
                jObject.Add(prop.Key, prop.Value.ToJsonNode());
            }
            return jObject;
        }
    }

    public class JsonArrayNode : JsonNode
    {
        public override JsonNodeType Type => JsonNodeType.Array;
        public List<JsonNode> Items { get; set; } = new List<JsonNode>();

        public override JsonNode Clone()
        {
            var clone = new JsonArrayNode { Parent = this.Parent };
            foreach (var item in Items)
            {
                clone.Items.Add(item.Clone());
            }
            return clone;
        }

        public override JsonNode ToJsonNode()
        {
            var jArray = new JsonArray();
            foreach (var item in Items)
            {
                jArray.Add(item.ToJsonNode());
            }
            return jArray;
        }
    }

    public class JsonValueNode : JsonNode
    {
        public override JsonNodeType Type => JsonNodeType.Value;
        public object? Value { get; set; }
        public JsonValueKind ValueKind { get; set; } = JsonValueKind.String;

        public override JsonNode Clone()
        {
            return new JsonValueNode { Parent = this.Parent, Value = this.Value, ValueKind = this.ValueKind };
        }

        public override JsonNode ToJsonNode()
        {
            if (Value == null) return JsonValue.Create(null);

            switch (ValueKind)
            {
                case JsonValueKind.String:
                    return JsonValue.Create(Value.ToString());
                case JsonValueKind.Number:
                    if (decimal.TryParse(Value.ToString(), out var decValue)) return JsonValue.Create(decValue);
                    return JsonValue.Create(0); // Fallback
                case JsonValueKind.True:
                    return JsonValue.Create(true);
                case JsonValueKind.False:
                    return JsonValue.Create(false);
                default:
                    return JsonValue.Create(Value.ToString());
            }
        }
    }
}
