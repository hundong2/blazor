using System.ComponentModel;

namespace BlazorAppLee.JsonPuzzleEditor.Models;

public class ConnectionPoint : INotifyPropertyChanged
{
    private bool _isConnected;
    private string? _connectedToId;

    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string PieceId { get; set; } = string.Empty;
    public ConnectionType Type { get; set; }
    public string Label { get; set; } = string.Empty;
    public double X { get; set; }
    public double Y { get; set; }

    public bool IsConnected
    {
        get => _isConnected;
        set
        {
            _isConnected = value;
            OnPropertyChanged(nameof(IsConnected));
        }
    }

    public string? ConnectedToId
    {
        get => _connectedToId;
        set
        {
            _connectedToId = value;
            OnPropertyChanged(nameof(ConnectedToId));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public enum ConnectionType
{
    Input,
    Output
}