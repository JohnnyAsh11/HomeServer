
using HomeServer.Formula1Api.DTOs;
using HomeServer.Formula1Database;
using HomeServer.Formula1Database.Models;
using InfluxDB.Client;
using System.Text.Json;

namespace HomeServer.Formula1Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // The URI to connect to OpenF1
            string uri = "https://api.openf1.org/v1/";

            // Configuring and building the web app.
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            builder.Host.ConfigureServices(ConfigureServices);

            WebApplication app = builder.Build();

            // Configuring the application.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Start();

            Formula1Repository repository = app.Services.GetRequiredService<Formula1Repository>();
            OpenF1Client f1Client = app.Services.GetRequiredService<OpenF1Client>();

            // Getting the active drivers since 2022 from the API. 
            List<DriverDto>? driverDtos = await f1Client.QueryApiAsync<List<DriverDto>>("drivers");
            if (driverDtos is null || driverDtos.Count == 0)
            {
                throw new Exception("There are no drivers which is not possible.");
            }

            // Sorting the drivers by their names to ensure no duplicates.
            driverDtos = [.. driverDtos.DistinctBy(driver => driver.BroadcastName)];

            // Creating a scope for the dbContext.
            AsyncServiceScope scope = app.Services.CreateAsyncScope();
            Formula1Context dbContext = scope.ServiceProvider.GetRequiredService<Formula1Context>();

            // Adding the data to the database.
            foreach (DriverDto driver in driverDtos)
            {
                await dbContext.AddAsync(DtoToDriverModel(driver));
            }

            // Saving the data and closing the scope.
            await dbContext.SaveChangesAsync();
            await scope.DisposeAsync();
        }

        /// <summary>
        /// Configures the settings for the TodoList Api application service.
        /// </summary>
        public static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            services.AddOpenApi();
            services.AddControllers();

            string uri = "https://api.openf1.org/v1/";
            services.AddHttpClient("OpenF1", client =>
            {
                client.BaseAddress = new Uri(uri);
            });

            services.AddSingleton<IInfluxDBClient>(_ =>
            {
                return new InfluxDBClient(
                    "http://localhost:8086",
                    "super-secret-token");
            });

            services.AddSingleton<OpenF1Client>();
            services.AddSingleton<Formula1Repository>();
            services.AddDbContext<Formula1Context>();
        }

        /// <summary>
        /// Converts the data from a racing lap to a telemetry entry.
        /// </summary>
        public static TelemetryEntry CreateTelemetryEntry(RaceLapDto raceLapDto)
        {
            return new TelemetryEntry
            {
                Timestamp = raceLapDto.Date!.Value,
                DriverNumber = raceLapDto.DriverNumber,
                Sector1 = raceLapDto.DurationSector1 ?? 0.0f,
                Sector2 = raceLapDto.DurationSector2 ?? 0.0f,
                Sector3 = raceLapDto.DurationSector3 ?? 0.0f,
                LapTime = raceLapDto.LapDuration ?? 0.0f,
                LapNumber = raceLapDto.LapNumber ?? 0,
                MeetingKey = raceLapDto.MeetingKey ?? 0,
                SessionKey = raceLapDto.SessionKey ?? 0
            };
        }

        public static F1DriverModel DtoToDriverModel(DriverDto dto)
        {
            return new F1DriverModel
            {
                BroadcastName = dto.BroadcastName,
                DriverName = dto.FullName,
                DriverNumber = dto.DriverNumber,
                NameAcronym = dto.NameAcronym,
                TeamColor = dto.TeamColor,
                TeamName = dto.TeamName,
                HeadshotUrl = dto.HeadshotUrl
            };
        }
    }
}
