namespace Qwirkle.WebApi.Client.Blazor.Services.Events;

public class PlayerTurnChangedEventArgs : EventArgs
{
    public string Pseudo { get; }

    public PlayerTurnChangedEventArgs(string pseudo)
    {
        Pseudo = pseudo;
    }
}