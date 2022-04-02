namespace Qwirkle.WebApi.Client.Blazor.Models;

public record BoardLimit
{
    public int MinX { get; private set; }
    public int MaxX { get; private set; }
    public int MinY { get; private set; }
    public int MaxY { get; private set; }

    public int LinesNumber => MaxY - MinY + 1;
    public int ColumnNumber => MaxX - MinX + 1;

    private readonly HashSet<Coordinate> _allCoordinates;

    public BoardLimit(IEnumerable<Coordinate> coordinates)
    {
        _allCoordinates = coordinates.ToHashSet();
        ComputeLimits();
    }

    public void Enlarge(Coordinate coordinate)
    {
        _allCoordinates.Add(coordinate);
        ComputeLimits();
    }

    public void Enlarge(IEnumerable<Coordinate> coordinates)
    {
        _allCoordinates.UnionWith(coordinates);
        ComputeLimits();
    }

    public void Reduce(Coordinate coordinate)
    {
        _allCoordinates.Remove(coordinate);
        ComputeLimits();
    }

    private void ComputeLimits()
    {
        if (_allCoordinates.Any())
        {
            MinX = _allCoordinates.Min(c => c.X) - 1;
            MaxX = _allCoordinates.Max(c => c.X) + 1;
            MinY = _allCoordinates.Min(c => c.Y) - 1;
            MaxY = _allCoordinates.Max(c => c.Y) + 1;
        }
        else
        {
            MinX = MinY = 0;
            MaxX = MaxY = 0;
        }
    }
}