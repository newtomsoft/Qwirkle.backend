namespace Qwirkle.WebApi.Client.Blazor.Models;

public class PlayerDetail
{
    public string Pseudo { get; }
    public int GamePosition { get; }
    public int Points { get; set; }
    public bool IsTurn { get; set; }

    public PlayerDetail(Player player)
    {
        Pseudo = player.Pseudo;
        GamePosition = player.GamePosition;
        Points = player.Points;
        IsTurn = player.IsTurn;
    }
}