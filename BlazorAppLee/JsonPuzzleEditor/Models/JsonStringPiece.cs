namespace BlazorAppLee.JsonPuzzleEditor.Models
{
    public class JsonStringPiece : PuzzlePiece
    {
        private string _value = "";
        
        public string Value 
        { 
            get => _value; 
            set 
            { 
                _value = value; 
                OnPropertyChanged(nameof(Value));
            } 
        }
        
        public JsonStringPiece()
        {
            Type = PuzzlePieceType.StringValue;
            OutputPoints.Add(new ConnectionPoint { Type = ConnectionType.Output });
        }
        
        public override object GetValue()
        {
            return Value;
        }
        
        public override void SetValue(object value)
        {
            Value = value?.ToString() ?? "";
        }
    }
}