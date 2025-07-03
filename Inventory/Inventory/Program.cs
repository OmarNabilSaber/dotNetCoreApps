using Inventory.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Inventory
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File($"Logs/Inventory-log-{DateTime.UtcNow:yyyyMMdd-HHmmss}-.log", rollingInterval: RollingInterval.Hour)
                .Enrich.FromLogContext()
                .CreateLogger();

            Log.Information("Starting Inventory application...");

            try
            {
                var builder = WebApplication.CreateBuilder(args);

                Log.Debug("Creating WebApplication builder...");

                // Add services to the container.
                builder.Services.AddControllersWithViews();
                Log.Debug("Added ControllersWithViews to services...");

                builder.Services.AddDbContext<AppDbContext>(options =>
                {
                    Log.Debug("Configuring DbContext options...");
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
                    Log.Debug("Using SQL Server connection string: {connectionString}", builder.Configuration.GetConnectionString("DefaultConnection"));
                });

                Log.Debug("Added DbContext to services...");

                var app = builder.Build();

                Log.Debug("Built WebApplication...");

                // Configure the HTTP request pipeline.
                if (!app.Environment.IsDevelopment())
                {
                    Log.Debug("Not in development environment, using exception handler...");
                    app.UseExceptionHandler("/Home/Error");
                }
                app.UseStaticFiles();
                Log.Debug("Using static files...");

                app.UseRouting();
                Log.Debug("Using routing...");

                app.UseAuthorization();
                Log.Debug("Using authorization...");

                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                Log.Debug("Mapped default controller route...");

                Log.Information("Starting WebApplication...");
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "An error occurred while starting the application");
                throw;
            }
        }
    }
}