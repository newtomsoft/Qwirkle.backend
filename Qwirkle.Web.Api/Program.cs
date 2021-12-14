using System.Configuration;

var appBuilder = WebApplication.CreateBuilder(args);
LogManager.Configuration = new NLogLoggingConfiguration(appBuilder.Configuration.GetSection("NLog"));
appBuilder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder => builder
    .SetIsOriginAllowed(_ => true)
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());
});
appBuilder.Services.AddSignalR();
appBuilder.Services.AddScoped<IRepository, Repository>();
appBuilder.Services.AddSingleton<INotification, SignalRNotification>();
appBuilder.Services.AddScoped<IAuthentication, Authentication>();
appBuilder.Services.AddScoped<AuthenticationUseCase>();
appBuilder.Services.AddScoped<CoreUseCase>();
appBuilder.Services.AddScoped<InfoUseCase>();
appBuilder.Services.AddScoped<BotUseCase>();
appBuilder.Services.AddScoped<IArtificialIntelligence, ArtificialIntelligence>();
appBuilder.Services.AddScoped<ComputePointsUseCase>();
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
app.UseCors("CorsPolicy");
#if DEBUG
app.UseDeveloperExceptionPage();
#else
app.UseExceptionHandler("/Home/Error");
#endif
app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<HubQwirkle>("/hubGame");
});
app.Run();