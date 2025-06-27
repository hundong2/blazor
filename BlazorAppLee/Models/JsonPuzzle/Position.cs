namespace BlazorAppLee.Models.JsonPuzzle;

public class Position
{
    public double X { get; set; }
    public double Y { get; set; }

    public Position(double x = 0, double y = 0)
    {
        X = x;
        Y = y;
    }
}