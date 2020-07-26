using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Qwirkle.Core.BagContext.Ports;
using Qwirkle.Core.BagContext.Services;
using Qwirkle.Core.ComplianceContext.Ports;
using Qwirkle.Infra.Persistance;
using Qwirkle.Infra.Persistance.Adapters;
using System;

namespace Qwirkle.Web.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IBagPersistance, BagPersistanceAdapter>();
            services.AddScoped<IRequestBagService, BagService>();

            services.AddScoped<ICompliancePersistance, CompliancePersistanceAdapter>();

            services.AddControllers();

            //services.AddDbContext<DefaultDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("UserDbContext")), ServiceLifetime.Scoped);
            services.AddDbContext<DefaultDbContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Scoped);

            //services.AddIdentity<TableUser, IdentityRole<int>>(options =>
            //{
            //    options.Password.RequireDigit = false;
            //    options.Password.RequireLowercase = false;
            //    options.Password.RequireNonAlphanumeric = false;
            //    options.Password.RequireUppercase = false;
            //    options.Password.RequiredLength = 6;
            //    options.Password.RequiredUniqueChars = 2;
            //})
            //.AddRoleManager<RoleManager<IdentityRole<int>>>()
            //.AddEntityFrameworkStores<DefaultContext>()
            //.AddDefaultTokenProviders()
            //.AddDefaultUI();

            services.AddOptions();
            //services.ConfigureApplicationCookie(options => options.LoginPath = "/Identity/Account/Login");
            //services.AddSession();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
