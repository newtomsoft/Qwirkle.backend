namespace Qwirkle.WebApi.Client.Blazor.Shared;

public partial class NavMenu
{
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private IdentityAuthenticationStateProvider AuthStateProvider { get; set; } = default!;

    private async Task LogoutClick()
    {
        await AuthStateProvider.Logout();
        NavigationManager.NavigateTo("/Login");
    }
}