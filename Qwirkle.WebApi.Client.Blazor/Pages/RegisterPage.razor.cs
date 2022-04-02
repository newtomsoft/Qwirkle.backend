namespace Qwirkle.WebApi.Client.Blazor.Pages;

public partial class RegisterPage : ComponentBase
{
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private IdentityAuthenticationStateProvider AuthStateProvider { get; set; } = default!;

    private RegisterModel RegisterModel { get; } = new() { SignInPersistent = true };
    private string Error { get; set; } = string.Empty;

    private async Task RegisterUser()
    {
        try
        {
            await AuthStateProvider!.Register(RegisterModel);
            NavigationManager!.NavigateTo(PageName.Home);
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
    }
}
