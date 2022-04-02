namespace Qwirkle.WebApi.Client.Blazor.Shared;

public partial class MainLayout
{
    private async Task LogoutClick()
    {
        await _authStateProvider.Logout();
        _navigationManager.NavigateTo("/login");
    }
}
