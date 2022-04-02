namespace Qwirkle.WebApi.Client.Blazor;

public static class AddServicesExtension
{
    public static void AddQwirkleMudServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddMudServices(config =>
        {
            config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopCenter;
            config.SnackbarConfiguration.PreventDuplicates = false;
            config.SnackbarConfiguration.NewestOnTop = false;
            config.SnackbarConfiguration.ShowCloseIcon = false;
            config.SnackbarConfiguration.VisibleStateDuration = 2200;
            config.SnackbarConfiguration.HideTransitionDuration = 400;
            config.SnackbarConfiguration.ShowTransitionDuration = 400;
            config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
        });
    }
}
