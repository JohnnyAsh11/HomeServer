using System.Threading.Tasks;

using HomeServer.Database;
using HomeServer.TodoList.Controllers;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Formatters;

using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

using Serilog;

namespace HomeServer.TodoList
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            builder.Host.ConfigureServices(ConfigureServices);
            builder.Host.UseSerilog((context, logging) => logging
                .ReadFrom.Configuration(builder.Configuration));

            WebApplication app = builder.Build();

            // app.UsePathBase("/api");
            app.UseHttpsRedirection();
            app.MapControllers();
            app.UseRouting();
            app.UseAuthorization();
            
            using (IServiceScope scope = app.Services.CreateScope())
            {
                HomeServerContext context = scope.ServiceProvider.GetRequiredService<HomeServerContext>();
                await context.Database.MigrateAsync();
            }
            app.Run();
        }

        /// <summary>
        /// Configures the settings for the TodoList Api application service.
        /// </summary>
        public static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            // General API setup.
            services.AddEndpointsApiExplorer();
            services.AddControllers();
            services.AddDbContext<HomeServerContext>();
            services.AddScoped<IController, TaskController>();

            // Open Telemetry
            OpenTelemetryBuilder otel = services.AddOpenTelemetry();
            services.AddLogging(logger => logger.AddOpenTelemetry(config =>
            {
                config.IncludeFormattedMessage = true;
                config.IncludeScopes = true;
            }));
            otel.WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation());
            otel.WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation());

            // Server health.
            services.AddHealthChecks();

            // Swagger
            // services.AddSwaggerGen();
        }
    }
}