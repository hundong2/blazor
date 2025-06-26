using BlazorAppLee.JsonPuzzleEditor.Models;
using System.Text.Json;

namespace BlazorAppLee.JsonPuzzleEditor.Services
{
    public class JsonBuilderService
    {
        public string BuildJson(List<PuzzlePiece> pieces)
        {
            try
            {
                var jsonObject = BuildJsonStructure(pieces);
                return JsonSerializer.Serialize(jsonObject, new JsonSerializerOptions 
                { 
                    WriteIndented = true 
                });
            }
            catch (Exception ex)
            {
                return $"{{ \"error\": \"{ex.Message}\" }}";
            }
        }
        
        private object BuildJsonStructure(List<PuzzlePiece> pieces)
        {
            // For now, create a simple structure from the pieces
            var result = new Dictionary<string, object>();
            
            var objectPieces = pieces.OfType<JsonObjectPiece>().ToList();
            var arrayPieces = pieces.OfType<JsonArrayPiece>().ToList();
            var stringPieces = pieces.OfType<JsonStringPiece>().ToList();
            var numberPieces = pieces.OfType<JsonNumberPiece>().ToList();
            var booleanPieces = pieces.OfType<JsonBooleanPiece>().ToList();

            if (stringPieces.Any())
            {
                result["strings"] = stringPieces.Select(p => p.GetValue()).ToArray();
            }
            
            if (numberPieces.Any())
            {
                result["numbers"] = numberPieces.Select(p => p.GetValue()).ToArray();
            }
            
            if (booleanPieces.Any())
            {
                result["booleans"] = booleanPieces.Select(p => p.GetValue()).ToArray();
            }

            if (result.Count == 0)
            {
                return new { message = "JSON puzzle editor working!", pieceCount = pieces.Count };
            }

            return result;
        }
    }
}