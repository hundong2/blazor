using System.Collections.Generic;
using System.Text.Json; // For potential use with System.Text.Json for parsing

namespace DataInspector.SharedComponents.Models
{
    public class JsonDisplayNode
    {
        public string? Key { get; set; }
        public object? Value { get; set; } // For simple types (string, number, boolean) or null
        public JsonDisplayNodeType NodeType { get; set; }
        public List<JsonDisplayNode> Children { get; set; } = new List<JsonDisplayNode>();

        // UI state for expanding/collapsing nodes
        public bool IsExpanded { get; set; } = true; // Default to expanded for better initial view

        // Helper to get string value for display, correctly formatting strings and booleans
        public string FormattedValue
        {
            get
            {
                if (NodeType == JsonDisplayNodeType.Null) return "null";
                if (Value == null) return "null"; // Should align with NodeType.Null generally

                switch (NodeType)
                {
                    case JsonDisplayNodeType.String:
                        // System.Text.Json.JsonSerializer can escape strings correctly if needed,
                        // but for display, often raw value is fine unless it contains special chars.
                        // For simplicity, showing as is, but ensure quotes are handled if Value already has them.
                        // Typically, Value for string node would be the unquoted string.
                        return $"\"{Value.ToString()}\"";
                    case JsonDisplayNodeType.Boolean:
                        return Value.ToString()?.ToLowerInvariant() ?? "null"; // "true" or "false"
                    case JsonDisplayNodeType.Number:
                        return Value.ToString() ?? string.Empty; // Numbers as is
                    default:
                        // This case should ideally not be hit if Value is only for primitives
                        return Value.ToString() ?? string.Empty;
                }
            }
        }

        // Constructor for convenience
        public JsonDisplayNode(string? key, JsonDisplayNodeType type, object? value = null)
        {
            Key = key;
            NodeType = type;
            if (type == JsonDisplayNodeType.Object || type == JsonDisplayNodeType.Array)
            {
                Children = new List<JsonDisplayNode>();
            }
            else
            {
                Value = value;
            }
        }

        // Default constructor for component parameter binding or other initializations
        public JsonDisplayNode() { }


        /// <summary>
        /// Parses a JSON string and converts it into a JsonDisplayNode tree.
        /// </summary>
        public static JsonDisplayNode? Parse(string? jsonString)
        {
            if (string.IsNullOrWhiteSpace(jsonString))
            {
                return null;
            }

            try
            {
                using (JsonDocument doc = JsonDocument.Parse(jsonString))
                {
                    return ConvertFromJsonElement(null, doc.RootElement);
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error deserializing JSON for tree view: {ex.Message}");
                // Return a single error node
                return new JsonDisplayNode("Error", JsonDisplayNodeType.String, $"Invalid JSON: {ex.Message.Split('.')[0]}");
            }
        }

        private static JsonDisplayNode ConvertFromJsonElement(string? key, JsonElement element)
        {
            JsonDisplayNodeType nodeType;
            object? value = null;
            var children = new List<JsonDisplayNode>();

            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    nodeType = JsonDisplayNodeType.Object;
                    foreach (JsonProperty property in element.EnumerateObject())
                    {
                        children.Add(ConvertFromJsonElement(property.Name, property.Value));
                    }
                    break;
                case JsonValueKind.Array:
                    nodeType = JsonDisplayNodeType.Array;
                    // Sort children by key if they are objects, or just add if not.
                    // For arrays, children usually don't have keys in this model, but if they represent complex objects, they might.
                    // For simplicity, adding in order.
                    foreach (JsonElement arrayElement in element.EnumerateArray())
                    {
                        children.Add(ConvertFromJsonElement(null, arrayElement)); // Array items don't have a key in this context
                    }
                    break;
                case JsonValueKind.String:
                    nodeType = JsonDisplayNodeType.String;
                    value = element.GetString();
                    break;
                case JsonValueKind.Number:
                    nodeType = JsonDisplayNodeType.Number;
                    if (element.TryGetInt64(out long l)) value = l;
                    else if (element.TryGetDouble(out double d)) value = d;
                    else if (element.TryGetDecimal(out decimal dec)) value = dec;
                    else value = element.GetRawText();
                    break;
                case JsonValueKind.True:
                    nodeType = JsonDisplayNodeType.Boolean;
                    value = true;
                    break;
                case JsonValueKind.False:
                    nodeType = JsonDisplayNodeType.Boolean;
                    value = false;
                    break;
                case JsonValueKind.Null:
                    nodeType = JsonDisplayNodeType.Null;
                    value = null;
                    break;
                default:
                    nodeType = JsonDisplayNodeType.String; // Fallback for undefined or other types
                    value = element.ToString();
                    break;
            }

            var node = new JsonDisplayNode(key, nodeType, value);
            if (children.Any()) // Check if children were added
            {
                node.Children = children;
            }
            return node;
        }
    }
}
