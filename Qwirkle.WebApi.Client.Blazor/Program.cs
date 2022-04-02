var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddQwirkleMudServices();
builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();
builder.Services.AddSingleton(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddSingleton<IdentityAuthenticationStateProvider>();
builder.Services.AddSingleton<AuthenticationStateProvider>(s => s.GetRequiredService<IdentityAuthenticationStateProvider>());
builder.Services.AddSingleton<IApiUser, WebApiUser>();
builder.Services.AddSingleton<IApiAction, WebApiAction>();
builder.Services.AddSingleton<IApiGame, WebApiGame>();
builder.Services.AddSingleton<IApiPlayer, WebApiPlayer>();
builder.Services.AddSingleton<IApiInstantGame, WebApiInstantGame>();
builder.Services.AddSingleton<INotificationInstantGame, SignalRNotificationInstantGame>();
builder.Services.AddSingleton<INotificationGame, SignalRNotificationGame>();
builder.Services.AddSingleton<IAreaManager, AreaManager>();
builder.Services.AddSingleton<IDragNDropManager, DragNDropManager>();
builder.Services.AddScoped<INotificationReceiver, NotificationReceiver>();
builder.Services.AddScoped<IPlayersDetail, PlayersDetail>();

var host = builder.Build();
var jsRuntime = host.Services.GetRequiredService<IJSRuntime>();
//todo get browser language


await host.RunAsync();