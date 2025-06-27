namespace DataInspector.SharedComponents.Models
{
    public class DataItem
    {
        // Assuming Number is more like a display row number or sequence,
        // while Id would be a unique key if needed for lookups or database.
        // For simplicity, let's use an Id that can be displayed or used as Number.
        public string Id { get; set; } = string.Empty; // Can be int if preferred, string offers flexibility
        public string Name { get; set; } = string.Empty;

        // Represents the value from the JSON, could be a simple string summary,
        // or a specific extracted value.
        public string ValueRepresentation { get; set; } = string.Empty;

        public ItemStatus Status { get; set; } = ItemStatus.None;

        // The full JSON string for this item, to be displayed in detail view.
        // This JSON structure is variable.
        public string DetailJsonString { get; set; } = "{}"; // Default to empty object JSON

        // Example constructor (optional)
        public DataItem() { }

        public DataItem(string id, string name, string valueRepresentation, ItemStatus status, string detailJson)
        {
            Id = id;
            Name = name;
            ValueRepresentation = valueRepresentation;
            Status = status;
            DetailJsonString = !string.IsNullOrWhiteSpace(detailJson) ? detailJson : "{}";
        }
    }
}
