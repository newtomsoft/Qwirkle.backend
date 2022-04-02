namespace Qwirkle.WebApi.Client.Blazor.Components;

public partial class LoginGuest : ComponentBase
{
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private IdentityAuthenticationStateProvider AuthStateProvider { get; set; } = default!;

    private async Task LoginAsGuest()
    {
        await AuthStateProvider.RegisterGuest();
        NavigationManager.NavigateTo(PageName.InstantGame);
    }
}
