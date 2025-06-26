using BlazorAppLee.JsonPuzzleEditor.Models;

namespace BlazorAppLee.JsonPuzzleEditor.Services
{
    public class DragDropService
    {
        public class DragData
        {
            public bool IsDragging { get; set; } = false;
            public double StartX { get; set; } = 0;
            public double StartY { get; set; } = 0;
            public double OffsetX { get; set; } = 0;
            public double OffsetY { get; set; } = 0;
            public PuzzlePiece? Element { get; set; }
        }

        public DragData CurrentDrag { get; private set; } = new();

        public event Action<PuzzlePiece, double, double>? OnPiecePositionChanged;

        public void StartDrag(PuzzlePiece piece, double clientX, double clientY)
        {
            CurrentDrag.IsDragging = true;
            CurrentDrag.Element = piece;
            CurrentDrag.StartX = clientX;
            CurrentDrag.StartY = clientY;
            CurrentDrag.OffsetX = clientX - piece.X;
            CurrentDrag.OffsetY = clientY - piece.Y;
        }

        public void UpdateDrag(double clientX, double clientY)
        {
            if (!CurrentDrag.IsDragging || CurrentDrag.Element == null) return;

            var newX = clientX - CurrentDrag.OffsetX;
            var newY = clientY - CurrentDrag.OffsetY;

            CurrentDrag.Element.X = newX;
            CurrentDrag.Element.Y = newY;
        }

        public void EndDrag(double clientX, double clientY)
        {
            if (!CurrentDrag.IsDragging || CurrentDrag.Element == null) return;

            var finalX = clientX - CurrentDrag.OffsetX;
            var finalY = clientY - CurrentDrag.OffsetY;

            CurrentDrag.Element.X = finalX;
            CurrentDrag.Element.Y = finalY;

            OnPiecePositionChanged?.Invoke(CurrentDrag.Element, finalX, finalY);

            CurrentDrag.IsDragging = false;
            CurrentDrag.Element = null;
        }
    }
}