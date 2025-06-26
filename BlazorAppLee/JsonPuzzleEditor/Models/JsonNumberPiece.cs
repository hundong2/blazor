namespace BlazorAppLee.JsonPuzzleEditor.Models
{
    public class JsonNumberPiece : PuzzlePiece
    {
        private double _value = 0;
        
        public double Value 
        { 
            get => _value; 
            set 
            { 
                _value = value; 
                OnPropertyChanged(nameof(Value));
            } 
        }
        
        public JsonNumberPiece()
        {
            Type = PuzzlePieceType.NumberValue;
            OutputPoints.Add(new ConnectionPoint { Type = ConnectionType.Output });
        }
        
        public override object GetValue()
        {
            return Value;
        }
        
        public override void SetValue(object value)
        {
            if (double.TryParse(value?.ToString(), out double result))
            {
                Value = result;
            }
        }
    }
}