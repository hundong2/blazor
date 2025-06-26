namespace BlazorAppLee.JsonPuzzleEditor.Models
{
    public class JsonObjectPiece : PuzzlePiece
    {
        public Dictionary<string, object> Properties { get; set; } = new();
        
        public JsonObjectPiece()
        {
            Type = PuzzlePieceType.JsonObject;
            // Object는 여러 입력을 받을 수 있음
            InputPoints.Add(new ConnectionPoint { Type = ConnectionType.Property });
            // Object는 하나의 출력을 가짐
            OutputPoints.Add(new ConnectionPoint { Type = ConnectionType.Output });
        }
        
        public override object GetValue()
        {
            return Properties;
        }
        
        public override void SetValue(object value)
        {
            if (value is Dictionary<string, object> dict)
            {
                Properties = dict;
            }
        }
        
        public void AddProperty(string key, object value)
        {
            Properties[key] = value;
            OnPropertyChanged(nameof(Properties));
        }
    }
}