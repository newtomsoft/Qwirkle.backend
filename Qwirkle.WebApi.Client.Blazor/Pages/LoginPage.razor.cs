namespace Qwirkle.WebApi.Client.Blazor.Pages;

public partial class LoginPage
{
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private IdentityAuthenticationStateProvider AuthStateProvider { get; set; } = default!;

    private LoginModel LoginModel { get; } = new() { RememberMe = true };
    private string Error { get; set; } = string.Empty;


    private async Task LoginAsUser()
    {
        try
        {
            await AuthStateProvider.Login(LoginModel);
            NavigationManager.NavigateTo(PageName.Home);
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
    }
}

