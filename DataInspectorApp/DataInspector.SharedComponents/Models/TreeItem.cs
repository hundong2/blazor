using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataInspector.SharedComponents.Models
{
    public class TreeItem
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public HashSet<TreeItem> Children { get; set; } = new HashSet<TreeItem>();

        // Using JsonIgnore to prevent serialization cycles if Parent is included.
        // This is good practice if you ever intend to serialize this model.
        [JsonIgnore]
        public TreeItem Parent { get; set; }

        public TreeItem(string id, string name, TreeItem parent = null)
        {
            Id = id;
            Name = name;
            Parent = parent;
        }

        // Parameterless constructor for serialization/deserialization
        public TreeItem() { }
    }
}
