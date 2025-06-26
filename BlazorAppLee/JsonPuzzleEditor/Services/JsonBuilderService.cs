using BlazorAppLee.JsonPuzzleEditor.Models;
using System.Text.Json;

namespace BlazorAppLee.JsonPuzzleEditor.Services;

public class JsonBuilderService
{
    private JsonStructure? _jsonStructure;

    public event Action<string>? JsonUpdated;
    public event Action<string>? ErrorOccurred;

    public void SetJsonStructure(JsonStructure jsonStructure)
    {
        _jsonStructure = jsonStructure;
    }

    public string BuildJson()
    {
        try
        {
            if (_jsonStructure == null || !_jsonStructure.Pieces.Any())
            {
                return "{}";
            }

            // Find root piece (piece with no input connections)
            var rootPiece = FindRootPiece();
            if (rootPiece == null)
            {
                return "{}";
            }

            var result = BuildJsonFromPiece(rootPiece);
            var json = JsonSerializer.Serialize(result, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            JsonUpdated?.Invoke(json);
            return json;
        }
        catch (Exception ex)
        {
            var errorMessage = $"Error building JSON: {ex.Message}";
            ErrorOccurred?.Invoke(errorMessage);
            return errorMessage;
        }
    }

    private PuzzlePiece? FindRootPiece()
    {
        if (_jsonStructure == null) return null;

        // Look for a piece that has no input connections
        foreach (var piece in _jsonStructure.Pieces)
        {
            var hasInputConnection = _jsonStructure.Connections.Any(c => 
                piece.InputPoints.Any(ip => ip.Id == c.ToPointId));
            
            if (!hasInputConnection && (piece is JsonObjectPiece || piece is JsonArrayPiece))
            {
                return piece;
            }
        }

        // If no clear root found, use the first object or array
        return _jsonStructure.Pieces.FirstOrDefault(p => p is JsonObjectPiece or JsonArrayPiece);
    }

    private object? BuildJsonFromPiece(PuzzlePiece piece)
    {
        if (_jsonStructure == null) return null;

        switch (piece)
        {
            case JsonObjectPiece objectPiece:
                return BuildObject(objectPiece);
            
            case JsonArrayPiece arrayPiece:
                return BuildArray(arrayPiece);
            
            case JsonPropertyPiece propertyPiece:
                return BuildProperty(propertyPiece);
            
            case JsonValuePiece valuePiece:
                return valuePiece.ToJsonValue();
            
            default:
                return null;
        }
    }

    private Dictionary<string, object?> BuildObject(JsonObjectPiece objectPiece)
    {
        var obj = new Dictionary<string, object?>();

        if (_jsonStructure == null) return obj;

        // Find all property pieces connected to this object
        var connectedProperties = FindConnectedPieces(objectPiece, typeof(JsonPropertyPiece));
        
        foreach (JsonPropertyPiece propertyPiece in connectedProperties.Cast<JsonPropertyPiece>())
        {
            var value = GetPropertyValue(propertyPiece);
            obj[propertyPiece.Key] = value;
        }

        return obj;
    }

    private List<object?> BuildArray(JsonArrayPiece arrayPiece)
    {
        var array = new List<object?>();

        if (_jsonStructure == null) return array;

        // Find all pieces connected to this array
        var connectedPieces = FindConnectedPieces(arrayPiece, null);
        
        foreach (var connectedPiece in connectedPieces)
        {
            var value = BuildJsonFromPiece(connectedPiece);
            array.Add(value);
        }

        return array;
    }

    private object? BuildProperty(JsonPropertyPiece propertyPiece)
    {
        // This shouldn't be called directly for building JSON
        // Properties are handled within BuildObject
        return GetPropertyValue(propertyPiece);
    }

    private object? GetPropertyValue(JsonPropertyPiece propertyPiece)
    {
        if (_jsonStructure == null) return null;

        // Find the value piece connected to this property's value input
        var valueConnection = _jsonStructure.Connections.FirstOrDefault(c =>
            propertyPiece.InputPoints.Any(ip => ip.Id == c.ToPointId && ip.Label == "Value"));

        if (valueConnection == null) return null;

        var valuePiece = _jsonStructure.FindPiece(valueConnection.FromPieceId);
        return valuePiece != null ? BuildJsonFromPiece(valuePiece) : null;
    }

    private List<PuzzlePiece> FindConnectedPieces(PuzzlePiece piece, Type? filterType)
    {
        var connectedPieces = new List<PuzzlePiece>();

        if (_jsonStructure == null) return connectedPieces;

        // Find connections from this piece's output points
        var outgoingConnections = _jsonStructure.Connections.Where(c =>
            piece.OutputPoints.Any(op => op.Id == c.FromPointId));

        foreach (var connection in outgoingConnections)
        {
            var connectedPiece = _jsonStructure.FindPiece(connection.ToPieceId);
            if (connectedPiece != null && (filterType == null || connectedPiece.GetType() == filterType))
            {
                connectedPieces.Add(connectedPiece);
            }
        }

        return connectedPieces;
    }

    public bool ValidateStructure()
    {
        if (_jsonStructure == null) return false;

        try
        {
            // Check for cycles
            if (HasCycles()) return false;

            // Check for valid connections
            foreach (var connection in _jsonStructure.Connections)
            {
                var fromPoint = _jsonStructure.FindConnectionPoint(connection.FromPointId);
                var toPoint = _jsonStructure.FindConnectionPoint(connection.ToPointId);

                if (fromPoint == null || toPoint == null) return false;
                if (fromPoint.Type != ConnectionType.Output) return false;
                if (toPoint.Type != ConnectionType.Input) return false;
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    private bool HasCycles()
    {
        if (_jsonStructure == null) return false;

        var visited = new HashSet<string>();
        var recursionStack = new HashSet<string>();

        foreach (var piece in _jsonStructure.Pieces)
        {
            if (HasCycleHelper(piece.Id, visited, recursionStack))
                return true;
        }

        return false;
    }

    private bool HasCycleHelper(string pieceId, HashSet<string> visited, HashSet<string> recursionStack)
    {
        if (_jsonStructure == null) return false;

        if (recursionStack.Contains(pieceId)) return true;
        if (visited.Contains(pieceId)) return false;

        visited.Add(pieceId);
        recursionStack.Add(pieceId);

        // Follow outgoing connections
        var piece = _jsonStructure.FindPiece(pieceId);
        if (piece != null)
        {
            var outgoingConnections = _jsonStructure.Connections.Where(c =>
                piece.OutputPoints.Any(op => op.Id == c.FromPointId));

            foreach (var connection in outgoingConnections)
            {
                if (HasCycleHelper(connection.ToPieceId, visited, recursionStack))
                    return true;
            }
        }

        recursionStack.Remove(pieceId);
        return false;
    }

    public void UpdateJson()
    {
        BuildJson();
    }
}