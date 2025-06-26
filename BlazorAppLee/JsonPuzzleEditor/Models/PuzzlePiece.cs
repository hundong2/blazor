using System.ComponentModel;

namespace BlazorAppLee.JsonPuzzleEditor.Models
{
    public abstract class PuzzlePiece : INotifyPropertyChanged
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public PuzzlePieceType Type { get; set; }
        public double X { get; set; } = 0;
        public double Y { get; set; } = 0;
        public bool IsSelected { get; set; } = false;
        public List<ConnectionPoint> InputPoints { get; set; } = new();
        public List<ConnectionPoint> OutputPoints { get; set; } = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public abstract object GetValue();
        public abstract void SetValue(object value);
    }
}