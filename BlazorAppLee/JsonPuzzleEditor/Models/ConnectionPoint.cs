namespace BlazorAppLee.JsonPuzzleEditor.Models
{
    public class ConnectionPoint
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public double X { get; set; }
        public double Y { get; set; }
        public ConnectionType Type { get; set; }
        public string? ConnectedToId { get; set; }
    }

    public enum ConnectionType
    {
        Input,
        Output,
        Property
    }
}