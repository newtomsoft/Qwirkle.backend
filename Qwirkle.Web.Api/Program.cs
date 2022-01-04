
using Qwirkle.Domain.UseCases.Ai;

var appBuilder = WebApplication.CreateBuilder(args);
LogManager.Configuration = new NLogLoggingConfiguration(appBuilder.Configuration.GetSection("NLog"));

const string underDevelopment = "CorsPolicyDevelopment";
const string underStagingOrProduction = "CorsPolicy";

var appBuilder = WebApplication.CreateBuilder(args);
appBuilder.Host.UseSerilog((_, configuration) => configuration.ReadFrom.Configuration(appBuilder.Configuration));
appBuilder.Services.AddCors(options =>
{
    // options.AddPolicy(underStagingOrProduction, builder => builder
    //         .WithOrigins("https://qwirkle.newtomsoft.fr", "http://qwirkle.newtomsoft.fr", "https://qwirkleapi.newtomsoft.fr", "http://qwirkleapi.newtomsoft.fr")
    //         .AllowCredentials()
    //         .AllowAnyHeader()
    //         .AllowAnyMethod()
    // );
    options.AddPolicy(underDevelopment, builder => builder
         .SetIsOriginAllowed(origin => true)
         .AllowCredentials()
         .AllowAnyHeader()
         .AllowAnyMethod()
 );
    // options.AddPolicy(underDevelopment, builder => builder
    //         .WithOrigins("https://localhost")
    //         .SetIsOriginAllowedToAllowWildcardSubdomains()
    //         .AllowAnyHeader()
    //         .AllowAnyMethod()
    //         .AllowCredentials()
    // );
});
appBuilder.Services.AddSignalR();
appBuilder.Services.AddSingleton<INotification, SignalRNotification>();
appBuilder.Services.AddScoped<IRepository, Repository>();
appBuilder.Services.AddScoped<IAuthentication, Authentication>();
appBuilder.Services.AddScoped<AuthenticationUseCase>();
appBuilder.Services.AddScoped<CoreUseCase>();
appBuilder.Services.AddScoped<InfoUseCase>();
appBuilder.Services.AddScoped<BotUseCase>();

appBuilder.Services.AddScoped<Expand>();
appBuilder.Services.AddScoped<IArtificialIntelligence, ArtificialIntelligence>();

appBuilder.Services.AddScoped<ComputePointsUseCase>();
appBuilder.Services.AddScoped<IArtificialIntelligence, ArtificialIntelligence>();
appBuilder.Services.AddControllers();
switch (appBuilder.Configuration.GetValue<string>("Repository").ToLowerInvariant())
{
    case "sqlserver":
        appBuilder.Services.AddDbContext<DefaultDbContext>(options => options.UseSqlServer(appBuilder.Configuration.GetConnectionString("Qwirkle")));
        break;
    case "postgres":
        appBuilder.Services.AddDbContext<DefaultDbContext>(options => options.UseNpgsql(appBuilder.Configuration.GetConnectionString("Qwirkle")));
        break;
}
appBuilder.Services.AddIdentity<UserDao, IdentityRole<int>>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 2;
    options.User.RequireUniqueEmail = true;
})
  .AddRoleManager<RoleManager<IdentityRole<int>>>()
  .AddEntityFrameworkStores<DefaultDbContext>()
  .AddDefaultTokenProviders()
  .AddDefaultUI();
appBuilder.Services.AddOptions();
appBuilder.Services.ConfigureApplicationCookie(options => options.LoginPath = "");
appBuilder.Services.AddSession();

var app = appBuilder.Build();

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(app.Environment.IsDevelopment() ? underDevelopment : underStagingOrProduction);
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<HubQwirkle>("/hubGame");
});
app.Run();