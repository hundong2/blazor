using BlazorAppLee.JsonPuzzleEditor.Models;
using Microsoft.JSInterop;

namespace BlazorAppLee.JsonPuzzleEditor.Services;

public class DragDropService
{
    private readonly IJSRuntime _jsRuntime;
    private JsonStructure? _jsonStructure;
    private PuzzlePiece? _draggedPiece;
    private bool _isDragging;
    private double _dragOffsetX;
    private double _dragOffsetY;

    public event Action<PuzzlePiece>? PiecePositionChanged;
    public event Action? CanvasUpdated;

    public DragDropService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public void SetJsonStructure(JsonStructure jsonStructure)
    {
        _jsonStructure = jsonStructure;
    }

    public async Task<bool> StartDrag(PuzzlePiece piece, double pointerX, double pointerY)
    {
        if (_isDragging) return false;

        _draggedPiece = piece;
        _isDragging = true;
        _dragOffsetX = pointerX - piece.X;
        _dragOffsetY = pointerY - piece.Y;

        piece.IsDragging = true;
        piece.IsSelected = true;

        // Update z-index for dragged piece
        await _jsRuntime.InvokeVoidAsync("puzzleEditor.startDrag", piece.Id);
        
        return true;
    }

    public async Task UpdateDrag(double pointerX, double pointerY)
    {
        if (!_isDragging || _draggedPiece == null) return;

        var newX = pointerX - _dragOffsetX;
        var newY = pointerY - _dragOffsetY;

        // Snap to grid (20px grid)
        newX = Math.Round(newX / 20) * 20;
        newY = Math.Round(newY / 20) * 20;

        // Boundary constraints
        newX = Math.Max(0, Math.Min(newX, 1200 - _draggedPiece.Width));
        newY = Math.Max(0, Math.Min(newY, 800 - _draggedPiece.Height));

        _draggedPiece.X = newX;
        _draggedPiece.Y = newY;

        PiecePositionChanged?.Invoke(_draggedPiece);
        CanvasUpdated?.Invoke();
    }

    public async Task EndDrag()
    {
        if (!_isDragging || _draggedPiece == null) return;

        _draggedPiece.IsDragging = false;
        await _jsRuntime.InvokeVoidAsync("puzzleEditor.endDrag", _draggedPiece.Id);

        _draggedPiece = null;
        _isDragging = false;
    }

    public bool IsDragging => _isDragging;
    public PuzzlePiece? DraggedPiece => _draggedPiece;

    public void SelectPiece(PuzzlePiece piece)
    {
        if (_jsonStructure == null) return;

        // Deselect all other pieces
        foreach (var p in _jsonStructure.Pieces)
        {
            p.IsSelected = false;
        }

        piece.IsSelected = true;
    }

    public void DeselectAll()
    {
        if (_jsonStructure == null) return;

        foreach (var piece in _jsonStructure.Pieces)
        {
            piece.IsSelected = false;
        }
    }

    public bool IsValidDropTarget(PuzzlePiece piece, double x, double y)
    {
        // Check if position is valid (not overlapping with other pieces)
        if (_jsonStructure == null) return true;

        var bounds = new { X = x, Y = y, Width = piece.Width, Height = piece.Height };

        foreach (var otherPiece in _jsonStructure.Pieces)
        {
            if (otherPiece.Id == piece.Id) continue;

            var otherBounds = new { X = otherPiece.X, Y = otherPiece.Y, Width = otherPiece.Width, Height = otherPiece.Height };

            // Check for overlap
            if (bounds.X < otherBounds.X + otherBounds.Width &&
                bounds.X + bounds.Width > otherBounds.X &&
                bounds.Y < otherBounds.Y + otherBounds.Height &&
                bounds.Y + bounds.Height > otherBounds.Y)
            {
                return false;
            }
        }

        return true;
    }

    public PuzzlePiece? CreatePieceFromTemplate(PuzzlePieceType type, double x, double y)
    {
        PuzzlePiece piece = type switch
        {
            PuzzlePieceType.JsonObject => new JsonObjectPiece(),
            PuzzlePieceType.JsonArray => new JsonArrayPiece(),
            PuzzlePieceType.JsonProperty => new JsonPropertyPiece(),
            PuzzlePieceType.JsonString => new JsonValuePiece(JsonValueType.String),
            PuzzlePieceType.JsonNumber => new JsonValuePiece(JsonValueType.Number),
            PuzzlePieceType.JsonBoolean => new JsonValuePiece(JsonValueType.Boolean),
            PuzzlePieceType.JsonNull => new JsonValuePiece(JsonValueType.Null),
            _ => throw new ArgumentException($"Unknown piece type: {type}")
        };

        piece.X = x;
        piece.Y = y;

        return piece;
    }
}