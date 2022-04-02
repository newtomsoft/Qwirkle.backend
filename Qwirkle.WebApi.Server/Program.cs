var builder = WebApplication.CreateBuilder(args);
var cors = new Cors();
builder.Configuration.GetSection("Cors").Bind(cors);
builder.Host.UseSerilog((_, configuration) => configuration.ReadFrom.Configuration(builder.Configuration));
builder.Services.AddDbContext<DefaultDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Qwirkle")));
builder.Services.AddSignalR();
builder.Services.AddResponseCompression(opts => opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" }));
builder.Services.AddSingleton<INotification, SignalRNotification>();
builder.Services.AddSingleton<InstantGameService>();
builder.Services.AddScoped<IRepository, Repository>();
builder.Services.AddScoped<IAuthentication, Authentication>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<CoreService>();
builder.Services.AddScoped<InfoService>();
builder.Services.AddScoped<BotService>();
builder.Services.AddControllers();
builder.Services.AddIdentity();
builder.Services.AddQwirkleCors(cors);
builder.Services.AddOptions();
builder.Services.ConfigureApplicationCookie(options => options.LoginPath = "/login");
builder.Services.AddSession();

var app = builder.Build();

app.UseQwirkleCors();
app.UseResponseCompression();

if (app.Environment.IsDevelopment()) app.UseWebAssemblyDebugging();
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.UseEndpoints(endpoints => { endpoints.MapControllers(); endpoints.MapHub<SignalRHub>("/hubGame"); });
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
