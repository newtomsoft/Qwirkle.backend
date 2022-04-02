namespace Qwirkle.WebApi.Client.Blazor.Tests.ComponentTests;

public static class SnackBarTestExtension
{
    public static void SnackBarMessageShouldBe(this TestContextBase context, string messageExpected, int messageIndex = 0)
        => context.Services.GetService<ISnackbar>()!.ShownSnackbars.ToArray()[messageIndex].Message.ShouldBe(messageExpected);

    public static void SnackBarMessageShouldContain(this TestContextBase context, string partExpected, int messageIndex = 0)
        => context.Services.GetService<ISnackbar>()!.ShownSnackbars.ToArray()[messageIndex].Message.ShouldContain(partExpected);
}