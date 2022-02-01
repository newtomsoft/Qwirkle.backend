namespace Qwirkle.Web.Api.ExtensionMethods;

public static class StartupExtensionMethods
{
    private const string CorsPolicyName = "CorsPolicy";

    public static void AddQwirkleCors(this IServiceCollection services, Cors cors) =>
        services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicyName, builder => builder
                .WithOrigins(cors.Origins)
                .AllowCredentials()
                .AllowAnyHeader()
                .AllowAnyMethod()
            );
        });

    public static void UseQwirkleCors(this WebApplication application) => application.UseCors(CorsPolicyName);

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
