using JsonPuzzleEditor.SharedComponents.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Encodings.Web; // For JavaScriptEncoder

namespace JsonPuzzleEditor.SharedComponents.Services
{
    public class JsonSerializerService
    {
        public string Serialize(JsonNodeData rootNode)
        {
            if (rootNode == null)
            {
                // Or throw an ArgumentNullException, depending on desired contract
                return "null";
            }

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                // Encoder = JavaScriptEncoder.Default // Default is fine for most cases
                // Using UnsafeRelaxedJsonEscaping can be useful if you know your content
                // and want to avoid excessive escaping, but be cautious with user-generated content.
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            try
            {
                object? jsonObjectModel = ConvertToJsonStandardTypes(rootNode);
                return JsonSerializer.Serialize(jsonObjectModel, options);
            }
            catch (Exception ex)
            {
                // Log the exception (e.g., using ILogger if injected)
                Console.WriteLine($"Error during JSON serialization: {ex.Message} - StackTrace: {ex.StackTrace}");
                // Return a more informative error JSON or rethrow as appropriate
                return JsonSerializer.Serialize(new { error = "Failed to serialize JSON", details = ex.Message }, options);
            }
        }

        private object? ConvertToJsonStandardTypes(JsonNodeData node)
        {
            switch (node.NodeType)
            {
                case JsonNodeType.Object:
                    var obj = new Dictionary<string, object?>();
                    foreach (var child in node.Children)
                    {
                        // Ensure key is not null or empty, though UI should prevent this for objects.
                        if (!string.IsNullOrEmpty(child.Key))
                        {
                            obj[child.Key] = ConvertToJsonStandardTypes(child);
                        }
                        else
                        {
                            // Handle cases where an object child might unexpectedly have no key
                            // This might indicate a data integrity issue or a bug in UI logic.
                            // For now, we'll skip it or assign a placeholder key.
                            // obj[$"missingKey_{Guid.NewGuid()}"] = ConvertToJsonStandardTypes(child);
                            Console.WriteLine($"Warning: Object child found with no key. Node value: {child.Value}");
                        }
                    }
                    return obj;

                case JsonNodeType.Array:
                    var arr = new List<object?>();
                    foreach (var child in node.Children)
                    {
                        arr.Add(ConvertToJsonStandardTypes(child));
                    }
                    return arr;

                case JsonNodeType.String:
                    return node.StringValue;

                case JsonNodeType.Number:
                    // Ensure the value is indeed a number.
                    // JsonNodeData.NumberValue tries to convert, but original Value might be more accurate.
                    if (node.Value is long longVal) return longVal;
                    if (node.Value is int intVal) return intVal;
                    if (node.Value is decimal decVal) return decVal; // JsonSerializer handles decimal well
                    if (node.Value is double doubleVal)
                    {
                        // Check for NaN or Infinity, which are not valid JSON numbers
                        if (double.IsNaN(doubleVal) || double.IsInfinity(doubleVal))
                        {
                            // Decide how to handle: throw, return null, or return a string placeholder
                            Console.WriteLine($"Warning: NaN or Infinity encountered for number node. Key: {node.Key}");
                            return "NaN_or_Infinity_not_allowed_in_JSON"; // Or null
                        }
                        // System.Text.Json handles double to number string conversion appropriately.
                        return doubleVal;
                    }
                    // Fallback or if NumberValue was the intended source
                    return node.NumberValue;

                case JsonNodeType.Boolean:
                    return node.BooleanValue;

                case JsonNodeType.Null:
                    return null;

                default:
                    // This should not happen if JsonNodeType enum is comprehensive
                    throw new ArgumentOutOfRangeException(nameof(node.NodeType), $"Unsupported node type: {node.NodeType}");
            }
        }

        // ****** DESERIALIZATION LOGIC ******
        public JsonNodeData? Deserialize(string jsonString)
        {
            if (string.IsNullOrWhiteSpace(jsonString))
            {
                // Return a default empty object node or null, based on desired behavior
                return new JsonNodeData { NodeType = JsonNodeType.Object, Key = null };
            }

            try
            {
                using (JsonDocument doc = JsonDocument.Parse(jsonString))
                {
                    // The root of a JSON document doesn't have a "key" in the traditional sense,
                    // unless it's being assigned as a child of another node.
                    // For the root node, the key is typically null.
                    return ConvertFromJsonElement(null, doc.RootElement);
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error deserializing JSON: {ex.Message}. Input: {jsonString.Substring(0, Math.Min(jsonString.Length, 100))}");
                // Optionally, create an error node to display in the UI
                var errorNode = new JsonNodeData { NodeType = JsonNodeType.Object, Key = "Error" };
                errorNode.Children.Add(new JsonNodeData { Key = "Message", NodeType = JsonNodeType.String, Value = $"Invalid JSON: {ex.Message}" });
                errorNode.Children.Add(new JsonNodeData { Key = "InputSnippet", NodeType = JsonNodeType.String, Value = jsonString.Substring(0, Math.Min(jsonString.Length,100)) });
                return errorNode;
            }
            catch (Exception ex) // Catch other potential errors during conversion
            {
                Console.WriteLine($"Unexpected error during deserialization: {ex.Message}. Input: {jsonString.Substring(0, Math.Min(jsonString.Length, 100))}");
                var errorNode = new JsonNodeData { NodeType = JsonNodeType.Object, Key = "Error" };
                errorNode.Children.Add(new JsonNodeData { Key = "Message", NodeType = JsonNodeType.String, Value = $"Deserialization failed: {ex.Message}" });
                return errorNode;
            }
        }

        private JsonNodeData ConvertFromJsonElement(string? key, JsonElement element)
        {
            JsonNodeType nodeType;
            object? value = null; // For primitive types
            List<JsonNodeData> children = new List<JsonNodeData>();

            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    nodeType = JsonNodeType.Object;
                    foreach (JsonProperty property in element.EnumerateObject())
                    {
                        children.Add(ConvertFromJsonElement(property.Name, property.Value));
                    }
                    break;
                case JsonValueKind.Array:
                    nodeType = JsonNodeType.Array;
                    foreach (JsonElement arrayElement in element.EnumerateArray())
                    {
                        // Array items in our JsonNodeData model don't have keys themselves.
                        // The 'key' parameter for ConvertFromJsonElement would be null here.
                        children.Add(ConvertFromJsonElement(null, arrayElement));
                    }
                    break;
                case JsonValueKind.String:
                    nodeType = JsonNodeType.String;
                    value = element.GetString();
                    break;
                case JsonValueKind.Number:
                    nodeType = JsonNodeType.Number;
                    // Attempt to preserve original number type if possible, though System.Text.Json often uses double/long/decimal
                    if (element.TryGetInt32(out int i)) value = i;
                    else if (element.TryGetInt64(out long l)) value = l;
                    else if (element.TryGetDouble(out double d)) value = d; // Could be Int64 if whole number
                    else if (element.TryGetDecimal(out decimal dec)) value = dec;
                    else value = element.GetRawText(); // Fallback, should ideally not be hit for valid numbers
                    break;
                case JsonValueKind.True:
                    nodeType = JsonNodeType.Boolean;
                    value = true;
                    break;
                case JsonValueKind.False:
                    nodeType = JsonNodeType.Boolean;
                    value = false;
                    break;
                case JsonValueKind.Null:
                    nodeType = JsonNodeType.Null;
                    value = null; // Explicitly set, though default for object is null
                    break;
                default: // Should not happen with valid JSON (Undefined, etc.)
                    Console.WriteLine($"Unsupported JsonValueKind: {element.ValueKind} for key '{key}'. Treating as string.");
                    nodeType = JsonNodeType.String;
                    value = element.ToString(); // Or GetRawText()
                    break;
            }

            return new JsonNodeData
            {
                Key = key,
                NodeType = nodeType,
                Value = value, // This will be null for Object and Array types
                Children = children // This will be empty for primitive types
            };
        }
    }
}
