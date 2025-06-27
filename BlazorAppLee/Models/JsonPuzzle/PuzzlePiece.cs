namespace BlazorAppLee.Models.JsonPuzzle;

public class PuzzlePiece
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public PuzzleType Type { get; set; }
    public Position Position { get; set; } = new();
    public List<ConnectionPoint> ConnectionPoints { get; set; } = new();
    public object? Value { get; set; }
    public string? Key { get; set; } // For property pieces
    public List<PuzzlePiece> Children { get; set; } = new(); // For objects and arrays
    public string? ParentId { get; set; }
    public bool IsSelected { get; set; }
    public bool IsDragging { get; set; }
}