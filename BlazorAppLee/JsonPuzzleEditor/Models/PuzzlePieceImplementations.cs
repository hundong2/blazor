namespace BlazorAppLee.JsonPuzzleEditor.Models
{
    public class JsonObjectPiece : PuzzlePiece
    {
        private Dictionary<string, object> _properties = new();

        public JsonObjectPiece()
        {
            Type = PuzzlePieceType.JsonObject;
            InputPoints.Add(new ConnectionPoint { Type = ConnectionType.Input });
            OutputPoints.Add(new ConnectionPoint { Type = ConnectionType.Output });
        }

        public override object GetValue()
        {
            return _properties;
        }

        public override void SetValue(object value)
        {
            if (value is Dictionary<string, object> dict)
            {
                _properties = dict;
            }
        }
    }

    public class JsonArrayPiece : PuzzlePiece
    {
        private List<object> _items = new();

        public JsonArrayPiece()
        {
            Type = PuzzlePieceType.JsonArray;
            InputPoints.Add(new ConnectionPoint { Type = ConnectionType.Input });
            OutputPoints.Add(new ConnectionPoint { Type = ConnectionType.Output });
        }

        public override object GetValue()
        {
            return _items;
        }

        public override void SetValue(object value)
        {
            if (value is List<object> list)
            {
                _items = list;
            }
        }
    }

    public class JsonStringPiece : PuzzlePiece
    {
        private string _value = "";

        public JsonStringPiece()
        {
            Type = PuzzlePieceType.StringValue;
            OutputPoints.Add(new ConnectionPoint { Type = ConnectionType.Output });
        }

        public string Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged(nameof(Value));
            }
        }

        public override object GetValue()
        {
            return _value;
        }

        public override void SetValue(object value)
        {
            if (value is string str)
            {
                Value = str;
            }
        }
    }

    public class JsonNumberPiece : PuzzlePiece
    {
        private double _value = 0;

        public JsonNumberPiece()
        {
            Type = PuzzlePieceType.NumberValue;
            OutputPoints.Add(new ConnectionPoint { Type = ConnectionType.Output });
        }

        public double Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged(nameof(Value));
            }
        }

        public override object GetValue()
        {
            return _value;
        }

        public override void SetValue(object value)
        {
            if (value is double d)
            {
                Value = d;
            }
            else if (value is int i)
            {
                Value = i;
            }
        }
    }

    public class JsonBooleanPiece : PuzzlePiece
    {
        private bool _value = false;

        public JsonBooleanPiece()
        {
            Type = PuzzlePieceType.BooleanValue;
            OutputPoints.Add(new ConnectionPoint { Type = ConnectionType.Output });
        }

        public bool Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged(nameof(Value));
            }
        }

        public override object GetValue()
        {
            return _value;
        }

        public override void SetValue(object value)
        {
            if (value is bool b)
            {
                Value = b;
            }
        }
    }
}