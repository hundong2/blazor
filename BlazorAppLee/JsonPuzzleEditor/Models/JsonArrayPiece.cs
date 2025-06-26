namespace BlazorAppLee.JsonPuzzleEditor.Models
{
    public class JsonArrayPiece : PuzzlePiece
    {
        public List<object> Items { get; set; } = new();
        
        public JsonArrayPiece()
        {
            Type = PuzzlePieceType.JsonArray;
            // Array는 여러 입력을 받을 수 있음
            InputPoints.Add(new ConnectionPoint { Type = ConnectionType.Input });
            // Array는 하나의 출력을 가짐
            OutputPoints.Add(new ConnectionPoint { Type = ConnectionType.Output });
        }
        
        public override object GetValue()
        {
            return Items;
        }
        
        public override void SetValue(object value)
        {
            if (value is List<object> list)
            {
                Items = list;
            }
        }
        
        public void AddItem(object item)
        {
            Items.Add(item);
            OnPropertyChanged(nameof(Items));
        }
    }
}