namespace JsonPuzzleEditor.SharedComponents.Models
{
    public class JsonNodeData
    {
        public string? Key { get; set; } // For object properties
        public object? Value { get; set; } // For simple types or null
        public JsonNodeType NodeType { get; set; } = JsonNodeType.String; // Default to string
        public List<JsonNodeData> Children { get; set; } = new List<JsonNodeData>();

        // Helper properties for typed values to avoid excessive casting in Razor
        public string StringValue
        {
            get => Value as string ?? string.Empty;
            set => Value = value;
        }

        public double NumberValue
        {
            get => Value is double d ? d : (Value is long l ? l : 0);
            set => Value = value;
        }

        public bool BooleanValue
        {
            get => Value is bool b && b;
            set => Value = value;
        }

        public JsonNodeData() { }

        public JsonNodeData(string? key, JsonNodeType type, object? value = null)
        {
            Key = key;
            NodeType = type;
            if (type == JsonNodeType.Object || type == JsonNodeType.Array)
            {
                Children = new List<JsonNodeData>();
            }
            else
            {
                Value = value;
            }
        }
    }
}
