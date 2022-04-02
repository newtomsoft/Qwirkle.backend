namespace Qwirkle.WebApi.Client.Blazor.Tests.ComponentTests;

public class LoginPageTests
{
    [Fact]
    public void LoginWithoutLoginAndPassword()
    {
        using var context = new TestContext();
        context.AddGenericServices();
        context.Services.AddQwirkleMudServices();
        var loginComponent = context.RenderComponent<LoginPage>();

        loginComponent.Find("#btnSignIn").Click();
        loginComponent.Find("#validationUserName").Text().ShouldNotBe("");
        loginComponent.Find("#validationPassword").Text().ShouldNotBe("");
    }


    [Fact]
    public void LoginAsRegisteredUser()
    {
        using var context = new TestContext();
        context.AddGenericServices();
        context.Services.AddQwirkleMudServices();
        var loginComponent = context.RenderComponent<LoginPage>();
        var navigationManager = context.Services.GetRequiredService<FakeNavigationManager>();

        loginComponent.Find("#inputUsername").Change("userTest");
        loginComponent.Find("#inputPassword").Change("passwordTest");
        loginComponent.Find("#btnSignIn").Click();
        var newUri = new Uri(navigationManager.Uri);
        newUri.LocalPath.ShouldBe(PageName.Home);
    }


    [Fact]
    public void LoginAsGuestShouldRedirectToInstantGame()
    {
        using var context = new TestContext();
        context.AddGenericServices();
        context.Services.AddQwirkleMudServices();
        var loginComponent = context.RenderComponent<LoginPage>();
        var navigationManager = context.Services.GetRequiredService<FakeNavigationManager>();

        loginComponent.Find("#btnLoginAsGuest").Click();
        var newUri = new Uri(navigationManager.Uri);
        newUri.LocalPath.ShouldBe(PageName.InstantGame);
    }
}