namespace Qwirkle.Web.Api.ExtensionMethods;

public static class StartupExtensionMethods
{
    private const string CorsPolicy = "CorsPolicy";

    public static void AddQwirkleCors(this IServiceCollection services, IConfigurationSection cors)
    {
        var origins = cors.GetSection("Origins").GetChildren().Select(e => e.Value).ToArray();
        services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicy, builder => builder
                .WithOrigins(origins)
                .AllowCredentials()
                .AllowAnyHeader()
                .AllowAnyMethod()
            );
        });
    }

    public static void UseQwirkleCors(this WebApplication application) => application.UseCors(CorsPolicy);

    public static void AddQwirkleIdentity(this IServiceCollection services) =>
        services.AddIdentity<UserDao, IdentityRole<int>>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 6;
            options.Password.RequiredUniqueChars = 2;
            options.User.RequireUniqueEmail = true;
        }).AddRoleManager<RoleManager<IdentityRole<int>>>()
          .AddEntityFrameworkStores<DefaultDbContext>()
          .AddDefaultTokenProviders()
          .AddDefaultUI();
}
