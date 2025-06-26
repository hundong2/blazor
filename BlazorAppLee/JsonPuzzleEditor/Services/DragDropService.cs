namespace BlazorAppLee.JsonPuzzleEditor.Services
{
    public class DragDropService
    {
        public event Action<string, double, double>? PiecePositionChanged;
        
        public void UpdatePiecePosition(string pieceId, double x, double y)
        {
            PiecePositionChanged?.Invoke(pieceId, x, y);
        }
        
        public (double X, double Y) SnapToGrid(double x, double y, int gridSize = 20)
        {
            return (
                Math.Round(x / gridSize) * gridSize,
                Math.Round(y / gridSize) * gridSize
            );
        }
    }
}