using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtomsoft.EntityFramework.Core;
using Qwikle.SignalR.Hubs;
using Qwirkle.Core.Ports;
using Qwirkle.Core.UsesCases;
using Qwirkle.Infra.Repository;
using Qwirkle.Infra.Repository.Adapters;
using Qwirkle.Infra.Repository.Dao;

namespace Qwirkle.Web.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // In general
            services.AddCors(c =>
            {
                c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyMethod()
                .AllowAnyHeader());
            });
            services.AddSignalR();
            services.AddScoped<IRepository, Repository>();
            services.AddScoped<CoreUseCase>();
            services.AddControllers();
            services.AddDbContext<DefaultDbContext>(Configuration);
            services.AddIdentity<UserDao, IdentityRole<int>>(options =>
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
          // Shows UseCors with CorsPolicyBuilder.
          app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
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
        }
    }
}
