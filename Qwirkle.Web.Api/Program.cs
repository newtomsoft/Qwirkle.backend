using Qwirkle.Web.Api.ExtensionMethods;

var appBuilder = WebApplication.CreateBuilder(args);
appBuilder.Host.UseSerilog((_, configuration) => configuration.ReadFrom.Configuration(appBuilder.Configuration));
appBuilder.Services.AddQwirkleCors(appBuilder.Configuration.GetSection("Cors"));
appBuilder.Services.AddSignalR();
appBuilder.Services.AddSingleton<INotification, SignalRNotification>();
appBuilder.Services.AddSingleton<InstantGameService>();
appBuilder.Services.AddScoped<IRepository, Repository>();
appBuilder.Services.AddScoped<IAuthentication, Authentication>();
appBuilder.Services.AddScoped<UserService>();
appBuilder.Services.AddScoped<CoreService>();
appBuilder.Services.AddScoped<InfoService>();
appBuilder.Services.AddScoped<BotService>();
appBuilder.Services.AddControllers();
appBuilder.Services.AddDbContext<DefaultDbContext>(options => options.UseSqlServer(appBuilder.Configuration.GetConnectionString("Qwirkle")));
appBuilder.Services.AddQwirkleIdentity();
appBuilder.Services.AddOptions();
appBuilder.Services.ConfigureApplicationCookie(options => options.LoginPath = "");
appBuilder.Services.AddSession();

var app = appBuilder.Build();
app.UseHttpsRedirection();
app.UseRouting();
app.UseQwirkleCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<HubQwirkle>("/hubGame");
});

app.Run();
