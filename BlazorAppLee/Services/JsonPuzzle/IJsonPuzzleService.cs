using BlazorAppLee.Models.JsonPuzzle;

namespace BlazorAppLee.Services.JsonPuzzle;

public interface IJsonPuzzleService
{
    string ConvertToJson(List<PuzzlePiece> puzzles);
    List<PuzzlePiece> ConvertFromJson(string json);
    bool ValidateConnection(PuzzlePiece source, PuzzlePiece target);
    PuzzlePiece CreatePuzzlePiece(PuzzleType type, object? value = null, string? key = null);
}