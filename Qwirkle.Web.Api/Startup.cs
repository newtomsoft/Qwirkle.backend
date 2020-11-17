using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtomsoft.Tools;
using Qwirkle.Core.ComplianceContext.Ports;
using Qwirkle.Core.ComplianceContext.Services;
using Qwirkle.Core.GameContext.Ports;
using Qwirkle.Core.GameContext.Services;
using Qwirkle.Core.PlayerContext.Ports;
using Qwirkle.Core.PlayerContext.Services;
using Qwirkle.Infra.Persistence;
using Qwirkle.Infra.Persistence.Adapters;
using Qwirkle.Infra.Persistence.Models;

namespace Qwirkle.Web.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ICompliancePersistence, CompliancePersistenceAdapter>();
            services.AddScoped<IGamePersistence, GamePersistenceAdapter>();
            services.AddScoped<IPlayerPersistence, PlayerPersistenceAdapter>();

            services.AddScoped<IRequestCompliance, ComplianceService>();
            services.AddScoped<IRequestGame, GameService>();
            services.AddScoped<IRequestPlayer, PlayerService>();

            services.AddControllers();

            EntityFrameworkTools<DefaultDbContext>.AddDbContext(services, Configuration);

            services.AddIdentity<UserPersistence, IdentityRole<int>>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 2;
            })
            .AddRoleManager<RoleManager<IdentityRole<int>>>()
            .AddEntityFrameworkStores<DefaultDbContext>()
            .AddDefaultTokenProviders()
            .AddDefaultUI();

            services.AddOptions();
            services.ConfigureApplicationCookie(options => options.LoginPath = "/Identity/Account/Login");
            services.AddSession();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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
            });
        }
    }
}
