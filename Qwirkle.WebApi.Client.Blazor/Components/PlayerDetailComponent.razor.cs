namespace Qwirkle.WebApi.Client.Blazor.Components;

public partial class PlayerDetailComponent
{
    [Parameter] public PlayerDetail PlayerDetail { get; set; } = default!;
}