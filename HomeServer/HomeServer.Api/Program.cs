using Hangfire;
using Hangfire.Dashboard;
using HomeServer.Api.Controllers;
using HomeServer.Database;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Serilog;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Hangfire.MemoryStorage;

namespace HomeServer.Api
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
            app.Use((context, next) =>
            {
                if (string.Equals(
                    context.Request.Headers["X-Forwarded-Prefix"].ToString(),
                    "/api",
                    StringComparison.Ordinal))
                {
                    context.Request.PathBase = "/api";
                }

                return next();
            });
            app.UseHangfireDashboard("/hangfire", new DashboardOptions()
            {
                Authorization = [new AllowAllDashboardAuthFilter()],
                IgnoreAntiforgeryToken = true,
            });

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
            services.AddSingleton<AiClientService>();
            services.AddSingleton<HangfireManager>();

            // API Endpoint Controllers:
            services.AddScoped<ITaskController, TodoController>();
            services.AddScoped<IChatController, AiChatController>();

            // Hangfire configuration:
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseMemoryStorage());
            services.AddHangfireServer();

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

            // Semantic Kernel Setup.
            string? aiModel = context.Configuration["OllamaAiModel"];
            if (aiModel is null)
            { 
                throw new Exception("Ollama AI model is not configured.");
            }

            IKernelBuilder kernel = services.AddKernel();
            services.AddOllamaChatCompletion(aiModel, new Uri("http://homeserver-ai:11434"));

            // Swagger
            // services.AddSwaggerGen();
        }

        /// <summary>
        /// Allows anyone access into the Hangfire dashboard.
        /// This is ok since it is an internal tool only accessible
        /// to me and those I provide permissions to through TailScale.
        /// </summary>
        internal class AllowAllDashboardAuthFilter : IDashboardAuthorizationFilter
        {
            public bool Authorize(DashboardContext context)
            {
                // Allow all users to access the dashboard
                return true;
            }
        }
    }
}