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
                if (!pieces.Any())
                {
                    return "{}";
                }
                
                var jsonObject = BuildJsonStructure(pieces);
                return JsonSerializer.Serialize(jsonObject, new JsonSerializerOptions 
                { 
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
            }
            catch (Exception ex)
            {
                return $"{{\n  \"error\": \"{ex.Message}\"\n}}";
            }
        }
        
        private object BuildJsonStructure(List<PuzzlePiece> pieces)
        {
            var result = new Dictionary<string, object>();
            
            // Find root object pieces (pieces without input connections)
            var rootPieces = pieces.Where(p => !HasInputConnections(p, pieces)).ToList();
            
            if (rootPieces.Count == 1 && rootPieces[0] is JsonObjectPiece rootObj)
            {
                return BuildObjectValue(rootObj, pieces);
            }
            else if (rootPieces.Count == 1 && rootPieces[0] is JsonArrayPiece rootArray)
            {
                return BuildArrayValue(rootArray, pieces);
            }
            else
            {
                // Multiple root pieces, create a container object
                for (int i = 0; i < rootPieces.Count; i++)
                {
                    var piece = rootPieces[i];
                    var key = $"item_{i + 1}";
                    result[key] = GetPieceValue(piece, pieces);
                }
            }
            
            return result;
        }
        
        private object BuildObjectValue(JsonObjectPiece objPiece, List<PuzzlePiece> allPieces)
        {
            var result = new Dictionary<string, object>();
            
            // For now, create sample properties based on connected pieces
            var connectedPieces = GetConnectedPieces(objPiece, allPieces);
            
            for (int i = 0; i < connectedPieces.Count; i++)
            {
                var piece = connectedPieces[i];
                var key = piece switch
                {
                    JsonStringPiece => "text",
                    JsonNumberPiece => "number", 
                    JsonArrayPiece => "array",
                    JsonObjectPiece => "object",
                    _ => $"property_{i + 1}"
                };
                
                result[key] = GetPieceValue(piece, allPieces);
            }
            
            return result;
        }
        
        private object BuildArrayValue(JsonArrayPiece arrayPiece, List<PuzzlePiece> allPieces)
        {
            var result = new List<object>();
            var connectedPieces = GetConnectedPieces(arrayPiece, allPieces);
            
            foreach (var piece in connectedPieces)
            {
                result.Add(GetPieceValue(piece, allPieces));
            }
            
            return result;
        }
        
        private object GetPieceValue(PuzzlePiece piece, List<PuzzlePiece> allPieces)
        {
            return piece switch
            {
                JsonStringPiece stringPiece => stringPiece.Value,
                JsonNumberPiece numberPiece => numberPiece.Value,
                JsonObjectPiece objPiece => BuildObjectValue(objPiece, allPieces),
                JsonArrayPiece arrayPiece => BuildArrayValue(arrayPiece, allPieces),
                _ => piece.GetValue()
            };
        }
        
        private bool HasInputConnections(PuzzlePiece piece, List<PuzzlePiece> allPieces)
        {
            // Simplified: for now, assume pieces without explicit parent connections are root
            return false;
        }
        
        private List<PuzzlePiece> GetConnectedPieces(PuzzlePiece parentPiece, List<PuzzlePiece> allPieces)
        {
            // Simplified: return pieces that are positioned near this piece (simulating connections)
            return allPieces.Where(p => p != parentPiece && 
                                  Math.Abs(p.X - parentPiece.X) < 200 && 
                                  Math.Abs(p.Y - parentPiece.Y) < 100)
                           .ToList();
        }
    }
}