using System.ComponentModel;

namespace BlazorAppLee.JsonPuzzleEditor.Models
{
    public abstract class PuzzlePiece : INotifyPropertyChanged
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        private double _x;
        private double _y;
        
        public double X 
        { 
            get => _x; 
            set 
            { 
                _x = value; 
                OnPropertyChanged(nameof(X));
            } 
        }
        
        public double Y 
        { 
            get => _y; 
            set 
            { 
                _y = value; 
                OnPropertyChanged(nameof(Y));
            } 
        }
        
        public PuzzlePieceType Type { get; set; }
        public List<ConnectionPoint> InputPoints { get; set; } = new();
        public List<ConnectionPoint> OutputPoints { get; set; } = new();
        public bool IsSelected { get; set; }
        public bool IsDragging { get; set; }
        
        public abstract object GetValue();
        public abstract void SetValue(object value);
        
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum PuzzlePieceType
    {
        JsonObject,
        JsonArray, 
        JsonProperty,
        StringValue,
        NumberValue,
        BooleanValue,
        NullValue
    }
}