namespace Qwirkle.WebApi.Client.Blazor.Tests.ComponentTests;

public class RegisterPageTests
{
    public static TheoryData<RegisterTestsInput> RegisterNewType => new()
    {
        new("", new RegisterModel { Email = "user@test.fr", UserName = "userName", Password = "password", PasswordConfirm = "password" }),
        new("#validationUserName", new RegisterModel { Email = "user@test.fr", Password = "password", PasswordConfirm = "password" }),
        new("#validationEmail", new RegisterModel { Email = "badEmailAddress", UserName = "userName", Password = "password", PasswordConfirm = "password" }),
        new("#validationEmail", new RegisterModel { UserName = "userName", Password = "password", PasswordConfirm = "password" }),
        new("#validationPassword", new RegisterModel { Email = "user@test.fr", UserName = "userName", PasswordConfirm = "password" }),
        new("#validationPassword", new RegisterModel { Email = "user@test.fr", UserName = "userName", Password = "tiny", PasswordConfirm = "tiny" }),
        new("#validationPasswordConfirm", new RegisterModel { Email = "user@test.fr", UserName = "userName", Password = "password1", PasswordConfirm = "password2" }),
    };

    [Theory(DisplayName = "Register")]
    [MemberData(nameof(RegisterNewType))]
    public void Register(RegisterTestsInput input)
    {
        using var context = new TestContext();
        context.AddGenericServices();
        context.Services.AddQwirkleMudServices();
        var registerPage = context.RenderComponent<RegisterPage>();
        var navigationManager = context.Services.GetRequiredService<FakeNavigationManager>();

        registerPage.Find("#inputUsername").Change(input.RegisterModel.UserName);
        registerPage.Find("#inputEmail").Change(input.RegisterModel.Email);
        registerPage.Find("#inputPassword").Change(input.RegisterModel.Password);
        registerPage.Find("#inputPasswordConfirm").Change(input.RegisterModel.PasswordConfirm);
        registerPage.Find("#btnRegister").Click();
        if (string.IsNullOrEmpty(input.InputSelectorInError))
        {
            var newUri = new Uri(navigationManager.Uri);
            newUri.LocalPath.ShouldBe(PageName.Home);
        }
        else
        {
            registerPage.Find(input.InputSelectorInError).Text().ShouldNotBe("");
        }
    }

    [Fact]
    public void RegisterAsGuestShouldRedirectToInstantGame()
    {
        using var context = new TestContext();
        context.AddGenericServices();
        context.Services.AddQwirkleMudServices();
        var loginComponent = context.RenderComponent<RegisterPage>();
        var navigationManager = context.Services.GetRequiredService<FakeNavigationManager>();

        loginComponent.Find("#btnLoginAsGuest").Click();
        var newUri = new Uri(navigationManager.Uri);
        newUri.LocalPath.ShouldBe(PageName.InstantGame);
    }
}
