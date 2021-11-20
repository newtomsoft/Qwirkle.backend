using Qwirkle.Authentication.Adapters;

var appBuilder = WebApplication.CreateBuilder(args);
var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
appBuilder.Configuration.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "..", "appsettings.json"), optional: true);
appBuilder.Configuration.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "..", $"appsettings.{environmentName}.json"), optional: true);
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
appBuilder.Services.AddControllers();
appBuilder.Services.AddDbContext<DefaultDbContext>(appBuilder.Configuration);
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