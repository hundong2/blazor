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
                if (stringPieces.Count == 1)
                {
                    result["textValue"] = stringPieces.First().GetValue();
                }
                else
                {
                    result["strings"] = stringPieces.Select(p => p.GetValue()).ToArray();
                }
            }
            
            if (numberPieces.Any())
            {
                if (numberPieces.Count == 1)
                {
                    result["numberValue"] = numberPieces.First().GetValue();
                }
                else
                {
                    result["numbers"] = numberPieces.Select(p => p.GetValue()).ToArray();
                }
            }
            
            if (booleanPieces.Any())
            {
                if (booleanPieces.Count == 1)
                {
                    result["booleanValue"] = booleanPieces.First().GetValue();
                }
                else
                {
                    result["booleans"] = booleanPieces.Select(p => p.GetValue()).ToArray();
                }
            }

            if (objectPieces.Any())
            {
                result["hasObjects"] = objectPieces.Count;
            }

            if (arrayPieces.Any())
            {
                result["hasArrays"] = arrayPieces.Count;
            }

            if (result.Count == 0)
            {
                return new { 
                    message = "JSON puzzle editor working!", 
                    instructions = "Add puzzle pieces from the toolbox to build JSON",
                    pieceCount = pieces.Count 
                };
            }

            result["totalPieces"] = pieces.Count;
            return result;
        }
    }
}