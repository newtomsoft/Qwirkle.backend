using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtomsoft.EntityFramework.Core;
using Qwirkle.Core.Ports;
using Qwirkle.Core.UsesCases;
using Qwirkle.Infra.Repository;
using Qwirkle.Infra.Repository.Adapters;
using Qwirkle.Infra.Repository.Dao;
using Qwirkle.SignalR;
using System;
using System.IO;

var builder = WebApplication.CreateBuilder(args);
var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
builder.Configuration.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "..", $"appsettings.{environmentName}.json"), optional: true); //When start with debugging
builder.Configuration.AddJsonFile($"appsettings.{environmentName}.json", optional: true); //When start without debugging
builder.Services.AddCors(options =>
    {
        options.AddPolicy("CorsPolicy", builder => builder
        .SetIsOriginAllowed((host) => true)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
    });
builder.Services.AddSignalR();
builder.Services.AddScoped<IRepository, Repository>();
builder.Services.AddScoped<CoreUseCase>();
builder.Services.AddControllers();
builder.Services.AddDbContext<DefaultDbContext>(builder.Configuration);
builder.Services.AddIdentity<UserDao, IdentityRole<int>>(options =>
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

builder.Services.AddOptions();
builder.Services.ConfigureApplicationCookie(options => options.LoginPath = "/Identity/Account/Login");
builder.Services.AddSession();

var app = builder.Build();

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


