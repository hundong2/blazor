namespace BlazorAppLee.JsonPuzzleEditor.Models;

public class JsonStructure
{
    public List<PuzzlePiece> Pieces { get; set; } = new();
    public List<Connection> Connections { get; set; } = new();
    public PuzzlePiece? RootPiece { get; set; }

    public void AddPiece(PuzzlePiece piece)
    {
        Pieces.Add(piece);
    }

    public void RemovePiece(string pieceId)
    {
        var piece = Pieces.FirstOrDefault(p => p.Id == pieceId);
        if (piece != null)
        {
            // Remove connections involving this piece
            Connections.RemoveAll(c => c.FromPieceId == pieceId || c.ToPieceId == pieceId);
            Pieces.Remove(piece);
        }
    }

    public void AddConnection(Connection connection)
    {
        // Remove any existing connection to the input point
        Connections.RemoveAll(c => c.ToPointId == connection.ToPointId);
        Connections.Add(connection);
    }

    public void RemoveConnection(string connectionId)
    {
        Connections.RemoveAll(c => c.Id == connectionId);
    }

    public Connection? FindConnection(string fromPointId, string toPointId)
    {
        return Connections.FirstOrDefault(c => c.FromPointId == fromPointId && c.ToPointId == toPointId);
    }

    public List<Connection> GetConnectionsForPiece(string pieceId)
    {
        return Connections.Where(c => c.FromPieceId == pieceId || c.ToPieceId == pieceId).ToList();
    }

    public PuzzlePiece? FindPiece(string pieceId)
    {
        return Pieces.FirstOrDefault(p => p.Id == pieceId);
    }

    public ConnectionPoint? FindConnectionPoint(string pointId)
    {
        foreach (var piece in Pieces)
        {
            var point = piece.InputPoints.Concat(piece.OutputPoints).FirstOrDefault(p => p.Id == pointId);
            if (point != null)
                return point;
        }
        return null;
    }
}

public class Connection
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string FromPieceId { get; set; } = string.Empty;
    public string FromPointId { get; set; } = string.Empty;
    public string ToPieceId { get; set; } = string.Empty;
    public string ToPointId { get; set; } = string.Empty;

    public double FromX { get; set; }
    public double FromY { get; set; }
    public double ToX { get; set; }
    public double ToY { get; set; }
}