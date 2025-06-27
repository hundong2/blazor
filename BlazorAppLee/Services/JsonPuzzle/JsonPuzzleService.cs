using BlazorAppLee.Models.JsonPuzzle;
using System.Text.Json;

namespace BlazorAppLee.Services.JsonPuzzle;

public class JsonPuzzleService : IJsonPuzzleService
{
    public string ConvertToJson(List<PuzzlePiece> puzzles)
    {
        if (!puzzles.Any())
            return "{}";

        var rootPieces = puzzles.Where(p => string.IsNullOrEmpty(p.ParentId)).ToList();
        
        if (rootPieces.Count == 1)
        {
            return ConvertPieceToJson(rootPieces.First(), puzzles);
        }
        
        // Multiple root pieces, wrap in object
        var result = new Dictionary<string, object>();
        foreach (var piece in rootPieces)
        {
            var key = piece.Key ?? $"item{rootPieces.IndexOf(piece)}";
            result[key] = ConvertPieceToJsonObject(piece, puzzles);
        }
        
        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }

    private string ConvertPieceToJson(PuzzlePiece piece, List<PuzzlePiece> allPieces)
    {
        var obj = ConvertPieceToJsonObject(piece, allPieces);
        return JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
    }

    private object ConvertPieceToJsonObject(PuzzlePiece piece, List<PuzzlePiece> allPieces)
    {
        switch (piece.Type)
        {
            case PuzzleType.Object:
                var obj = new Dictionary<string, object>();
                var children = allPieces.Where(p => p.ParentId == piece.Id).ToList();
                foreach (var child in children)
                {
                    var key = child.Key ?? $"property{children.IndexOf(child)}";
                    obj[key] = ConvertPieceToJsonObject(child, allPieces);
                }
                return obj;

            case PuzzleType.Array:
                var array = new List<object>();
                var arrayChildren = allPieces.Where(p => p.ParentId == piece.Id).OrderBy(p => p.Position.Y).ToList();
                foreach (var child in arrayChildren)
                {
                    array.Add(ConvertPieceToJsonObject(child, allPieces));
                }
                return array;

            case PuzzleType.String:
                return piece.Value?.ToString() ?? "";

            case PuzzleType.Number:
                if (double.TryParse(piece.Value?.ToString(), out var number))
                    return number;
                return 0;

            case PuzzleType.Boolean:
                if (bool.TryParse(piece.Value?.ToString(), out var boolean))
                    return boolean;
                return false;

            case PuzzleType.Null:
                return null!;

            default:
                return piece.Value ?? "";
        }
    }

    public List<PuzzlePiece> ConvertFromJson(string json)
    {
        var pieces = new List<PuzzlePiece>();
        
        try
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(json);
            ConvertJsonElementToPieces(jsonElement, pieces, null, null);
        }
        catch
        {
            // Return empty list if JSON is invalid
        }
        
        return pieces;
    }

    private void ConvertJsonElementToPieces(JsonElement element, List<PuzzlePiece> pieces, string? parentId, string? key)
    {
        PuzzlePiece piece;
        
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                piece = CreatePuzzlePiece(PuzzleType.Object, null, key);
                piece.ParentId = parentId;
                pieces.Add(piece);
                
                foreach (var property in element.EnumerateObject())
                {
                    ConvertJsonElementToPieces(property.Value, pieces, piece.Id, property.Name);
                }
                break;

            case JsonValueKind.Array:
                piece = CreatePuzzlePiece(PuzzleType.Array, null, key);
                piece.ParentId = parentId;
                pieces.Add(piece);
                
                var index = 0;
                foreach (var item in element.EnumerateArray())
                {
                    ConvertJsonElementToPieces(item, pieces, piece.Id, index.ToString());
                    index++;
                }
                break;

            case JsonValueKind.String:
                piece = CreatePuzzlePiece(PuzzleType.String, element.GetString(), key);
                piece.ParentId = parentId;
                pieces.Add(piece);
                break;

            case JsonValueKind.Number:
                piece = CreatePuzzlePiece(PuzzleType.Number, element.GetDouble(), key);
                piece.ParentId = parentId;
                pieces.Add(piece);
                break;

            case JsonValueKind.True:
            case JsonValueKind.False:
                piece = CreatePuzzlePiece(PuzzleType.Boolean, element.GetBoolean(), key);
                piece.ParentId = parentId;
                pieces.Add(piece);
                break;

            case JsonValueKind.Null:
                piece = CreatePuzzlePiece(PuzzleType.Null, null, key);
                piece.ParentId = parentId;
                pieces.Add(piece);
                break;
        }
    }

    public bool ValidateConnection(PuzzlePiece source, PuzzlePiece target)
    {
        // Basic validation rules
        if (source.Id == target.Id)
            return false;

        // Objects and arrays can contain other pieces
        if (source.Type == PuzzleType.Object || source.Type == PuzzleType.Array)
        {
            return true;
        }

        // Prevent circular connections
        if (IsCircularConnection(source, target))
            return false;

        return true;
    }

    private bool IsCircularConnection(PuzzlePiece source, PuzzlePiece target)
    {
        var current = target;
        while (!string.IsNullOrEmpty(current.ParentId))
        {
            if (current.ParentId == source.Id)
                return true;
            // This would need access to all pieces to traverse the hierarchy
            break;
        }
        return false;
    }

    public PuzzlePiece CreatePuzzlePiece(PuzzleType type, object? value = null, string? key = null)
    {
        var piece = new PuzzlePiece
        {
            Type = type,
            Value = value,
            Key = key
        };

        // Add default connection points based on type
        switch (type)
        {
            case PuzzleType.Object:
                piece.ConnectionPoints.Add(new ConnectionPoint
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = "input",
                    RelativePosition = new Position(0, 10)
                });
                piece.ConnectionPoints.Add(new ConnectionPoint
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = "output",
                    RelativePosition = new Position(100, 50)
                });
                break;

            case PuzzleType.Array:
                piece.ConnectionPoints.Add(new ConnectionPoint
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = "input",
                    RelativePosition = new Position(0, 10)
                });
                piece.ConnectionPoints.Add(new ConnectionPoint
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = "output",
                    RelativePosition = new Position(100, 50)
                });
                break;

            default:
                piece.ConnectionPoints.Add(new ConnectionPoint
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = "input",
                    RelativePosition = new Position(0, 10)
                });
                break;
        }

        return piece;
    }
}