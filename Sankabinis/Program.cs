using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Sankabinis.Data;
using Sankabinis.Controllers; // Assuming GoogleApiController is in a namespace called YourNamespace.GoogleApi

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure configuration services
var configBuilder = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("%APPDATA%\\Microsoft\\UserSecrets\\4012a855-361e-424b-bd4f-a21b60aaf802\\secrets.json", optional: true, reloadOnChange: true);

// Build the configuration
var configuration = configBuilder.Build();

// Configure database context
builder.Services.AddDbContext<SankabinisContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("SankabinisContext") ?? throw new InvalidOperationException("Connection string 'SankabinisContext' not found.")));

// Add session state
builder.Services.AddSession();

// Register GoogleApiController
builder.Services.AddScoped<GoogleApiController>(); // Assuming GoogleApiController has no dependencies and can be created per request

//SignalR
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
