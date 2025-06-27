namespace BlazorAppLee.Models.JsonPuzzle;

public class ConnectionPoint
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "input" or "output"
    public Position RelativePosition { get; set; } = new();
    public bool IsConnected { get; set; }
    public string? ConnectedTo { get; set; }
}