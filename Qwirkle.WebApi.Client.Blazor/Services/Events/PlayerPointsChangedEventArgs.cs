namespace Qwirkle.WebApi.Client.Blazor.Services.Events;

public class PlayerPointsChangedEventArgs : EventArgs
{
    public string Pseudo { get; }
    public int Points { get; set; }

    public PlayerPointsChangedEventArgs(string pseudo, int points)
    {
        Pseudo = pseudo;
        Points = points;
    }
}