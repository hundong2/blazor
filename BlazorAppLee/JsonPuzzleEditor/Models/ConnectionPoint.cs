namespace BlazorAppLee.JsonPuzzleEditor.Models
{
    public class ConnectionPoint
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public ConnectionType Type { get; set; }
        public string? ConnectedToId { get; set; }
    }
}