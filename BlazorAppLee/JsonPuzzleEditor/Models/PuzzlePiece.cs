using System.ComponentModel;

namespace BlazorAppLee.JsonPuzzleEditor.Models;

public abstract class PuzzlePiece : INotifyPropertyChanged
{
    private double _x;
    private double _y;
    private bool _isSelected;
    private bool _isDragging;

    public string Id { get; set; } = Guid.NewGuid().ToString();
    public abstract PuzzlePieceType Type { get; }
    public List<ConnectionPoint> InputPoints { get; set; } = new();
    public List<ConnectionPoint> OutputPoints { get; set; } = new();

    public double X
    {
        get => _x;
        set
        {
            _x = value;
            OnPropertyChanged(nameof(X));
            UpdateConnectionPoints();
        }
    }

    public double Y
    {
        get => _y;
        set
        {
            _y = value;
            OnPropertyChanged(nameof(Y));
            UpdateConnectionPoints();
        }
    }

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            OnPropertyChanged(nameof(IsSelected));
        }
    }

    public bool IsDragging
    {
        get => _isDragging;
        set
        {
            _isDragging = value;
            OnPropertyChanged(nameof(IsDragging));
        }
    }

    public double Width { get; set; } = 150;
    public double Height { get; set; } = 80;

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected virtual void UpdateConnectionPoints()
    {
        // Update connection point positions relative to piece position
        foreach (var point in InputPoints.Concat(OutputPoints))
        {
            point.X = X + (point.Type == ConnectionType.Input ? 0 : Width);
            point.Y = Y + Height / 2;
        }
    }

    public abstract object? ToJsonValue();
}

public enum PuzzlePieceType
{
    JsonObject,
    JsonArray,
    JsonProperty,
    JsonString,
    JsonNumber,
    JsonBoolean,
    JsonNull
}

public class JsonObjectPiece : PuzzlePiece
{
    public override PuzzlePieceType Type => PuzzlePieceType.JsonObject;
    public List<string> PropertyIds { get; set; } = new();

    public JsonObjectPiece()
    {
        InputPoints.Add(new ConnectionPoint { Id = Guid.NewGuid().ToString(), PieceId = Id, Type = ConnectionType.Input, Label = "Object" });
        OutputPoints.Add(new ConnectionPoint { Id = Guid.NewGuid().ToString(), PieceId = Id, Type = ConnectionType.Output, Label = "Object" });
        Height = 100;
    }

    public override object? ToJsonValue()
    {
        var obj = new Dictionary<string, object?>();
        // Properties will be added by connected property pieces
        return obj;
    }
}

public class JsonArrayPiece : PuzzlePiece
{
    public override PuzzlePieceType Type => PuzzlePieceType.JsonArray;
    public List<string> ElementIds { get; set; } = new();

    public JsonArrayPiece()
    {
        InputPoints.Add(new ConnectionPoint { Id = Guid.NewGuid().ToString(), PieceId = Id, Type = ConnectionType.Input, Label = "Array" });
        OutputPoints.Add(new ConnectionPoint { Id = Guid.NewGuid().ToString(), PieceId = Id, Type = ConnectionType.Output, Label = "Array" });
        Height = 80;
    }

    public override object? ToJsonValue()
    {
        var array = new List<object?>();
        // Elements will be added by connected pieces
        return array;
    }
}

public class JsonPropertyPiece : PuzzlePiece
{
    private string _key = "key";

    public override PuzzlePieceType Type => PuzzlePieceType.JsonProperty;

    public string Key
    {
        get => _key;
        set
        {
            _key = value;
            OnPropertyChanged(nameof(Key));
        }
    }

    public JsonPropertyPiece()
    {
        InputPoints.Add(new ConnectionPoint { Id = Guid.NewGuid().ToString(), PieceId = Id, Type = ConnectionType.Input, Label = "Object" });
        InputPoints.Add(new ConnectionPoint { Id = Guid.NewGuid().ToString(), PieceId = Id, Type = ConnectionType.Input, Label = "Value" });
        OutputPoints.Add(new ConnectionPoint { Id = Guid.NewGuid().ToString(), PieceId = Id, Type = ConnectionType.Output, Label = "Property" });
        Width = 180;
        Height = 90;
    }

    public override object? ToJsonValue()
    {
        return new KeyValuePair<string, object?>(Key, null); // Value will be set by connected piece
    }
}

public class JsonValuePiece : PuzzlePiece
{
    private object? _value;
    private JsonValueType _valueType = JsonValueType.String;

    public override PuzzlePieceType Type => PuzzlePieceType.JsonString; // Will be updated based on ValueType

    public object? Value
    {
        get => _value;
        set
        {
            _value = value;
            OnPropertyChanged(nameof(Value));
        }
    }

    public JsonValueType ValueType
    {
        get => _valueType;
        set
        {
            _valueType = value;
            OnPropertyChanged(nameof(ValueType));
            UpdateValue();
        }
    }

    public JsonValuePiece(JsonValueType valueType = JsonValueType.String)
    {
        ValueType = valueType;
        InputPoints.Add(new ConnectionPoint { Id = Guid.NewGuid().ToString(), PieceId = Id, Type = ConnectionType.Input, Label = "Container" });
        OutputPoints.Add(new ConnectionPoint { Id = Guid.NewGuid().ToString(), PieceId = Id, Type = ConnectionType.Output, Label = "Value" });
        Width = 120;
        Height = 60;
        UpdateValue();
    }

    private void UpdateValue()
    {
        Value = ValueType switch
        {
            JsonValueType.String => "text",
            JsonValueType.Number => 0,
            JsonValueType.Boolean => false,
            JsonValueType.Null => null,
            _ => "text"
        };
    }

    public override object? ToJsonValue()
    {
        return Value;
    }
}

public enum JsonValueType
{
    String,
    Number,
    Boolean,
    Null
}