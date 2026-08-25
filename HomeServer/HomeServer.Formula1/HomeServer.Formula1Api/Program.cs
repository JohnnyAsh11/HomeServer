
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

            HttpClient client = app.Services
                .GetRequiredService<IHttpClientFactory>()
                .CreateClient("OpenF1");

            Formula1Repository repository = app.Services.GetRequiredService<Formula1Repository>();


            /*
             
            All sessions for a given year:
            https://api.openf1.org/v1/sessions?date_start%3E%3D2023-01-01&date_end%3C%3D2023-12-10

            All tire stints for a given race:
            https://api.openf1.org/v1/stints?session_key=9165

            All sessions within a meeting:
            https://api.openf1.org/v1/sessions?meeting_key=1216

            Weather data for a given session:
            https://api.openf1.org/v1/weather?meeting_key=1216&session_key=9286
             
             */


            for (int i = 1; i < 44; i++)
            {
                //HttpResponseMessage resp = await client.GetAsync($"intervals?session_key=11334&interval<{i}");
                HttpResponseMessage resp = await client.GetAsync($"laps?session_key=11334&lap_number={i}");

                if (resp.Content is null)
                {
                    throw new Exception("Failed to retrieve lap data.");
                }

                string json = await resp.Content.ReadAsStringAsync();
                Console.WriteLine(json);
                List<RaceLapDto>? raceLapDtos = JsonSerializer.Deserialize<List<RaceLapDto>>(json);

                if (raceLapDtos is null)
                {
                    throw new Exception("Failed to deserialize race lap data.");
                }

                foreach (RaceLapDto raceLapDto in raceLapDtos)
                {
                    await repository.SaveAsync(CreateTelemetryEntry(raceLapDto));
                }

                // Rate limiting.
                await Task.Delay(2000);
            }
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

            //lzZv3ncJgqIrI1IvkSEbKgNkhLkNKcDGpnOkB0kxYHPEonjTPX3OaE-XGeFwCrJZ3bpODxCiaiCf09azgHdi0w==
            services.AddSingleton<IInfluxDBClient>(_ =>
            {
                return new InfluxDBClient(
                    "http://localhost:8086",
                    "super-secret-token");
            });

            services.AddSingleton<Formula1Repository>();
        }

        public static TelemetryEntry CreateTelemetryEntry(RaceLapDto raceLapDto)
        {
            return new TelemetryEntry
            {
                Timestamp = DateTime.UtcNow,
                DriverNumber = raceLapDto.DriverNumber,
                Sector1 = raceLapDto.DurationSector1 ?? 0.0f,
                Sector2 = raceLapDto.DurationSector2 ?? 0.0f,
                Sector3 = raceLapDto.DurationSector3 ?? 0.0f,
                LapTime = raceLapDto.LapDuration ?? 0.0f,
                LapNumber = raceLapDto.LapNumber,
                MeetingKey = raceLapDto.MeetingKey,
                SessionKey = raceLapDto.SessionKey
            };
        }
    }
}
