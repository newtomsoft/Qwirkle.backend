namespace Qwirkle.WebApi.Client.Blazor.Services.Events;

public class TileOutBoardDroppedEventArgs : EventArgs
{
    public Coordinate Coordinate { get; set; }
}