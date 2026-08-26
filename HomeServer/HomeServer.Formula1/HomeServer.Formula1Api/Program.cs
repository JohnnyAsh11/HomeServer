
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

            // Reading in the session data.
            for (int i = 2026; i < 2027; i++)
            {
                Console.WriteLine($" - - - - - - Races for the year {i} - - - - - -");
                //HttpResponseMessage resp = await client.GetAsync($"intervals?session_key=11334&interval<{i}");
                HttpResponseMessage respSessions = await client.GetAsync($"sessions?date_start%3E%3D{i}-01-01&date_end%3C%3D{i}-12-10");

                if (respSessions.Content is null)
                {
                    throw new Exception($"Failed to retrieve session data from the year {i}.");
                }

                // This is relatively safe deserialization since if there are session
                // keys returned then the sessions exist in the API.
                string json = await respSessions.Content.ReadAsStringAsync();
                List<SessionDto>? sessionDtos = JsonSerializer.Deserialize<List<SessionDto>>(json);

                if (sessionDtos is null)
                {
                    throw new Exception("Failed to deserialize session data.");
                }

                // Getting the lap timing data for each session.
                foreach (SessionDto sessionDto in sessionDtos)
                {
                    Console.WriteLine($"{sessionDto.SessionKey}: {sessionDto.CircuitShortName} {sessionDto.SessionName}");
                    HttpResponseMessage respLap = await client.GetAsync($"laps?session_key={sessionDto.SessionKey}");

                    if (respLap.Content is null)
                    {
                        throw new Exception($"Failed to retrieve lap data for session {sessionDto.SessionKey}.");
                    }

                    string lapJson = await respLap.Content.ReadAsStringAsync();
                    List<RaceLapDto>? lapDtos = null;

                    // Attempt to deserialize the lap data.
                    // NOTE: There are many failure conditions:
                    // - Laps may be incomplete due to DNFs
                    // - Laps may be incomplete due to race cancellations
                    // - Laps may be incomplete because the session has not yet happened.
                    try
                    {
                        lapDtos = JsonSerializer.Deserialize<List<RaceLapDto>>(lapJson);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error deserializing lap data for session {sessionDto.SessionKey}: {ex.Message}");

                        ErrorDto? err = JsonSerializer.Deserialize<ErrorDto>(lapJson);
                        
                        // If the API returned the error, continue onward.
                        if (err is not null)
                        {
                            continue;
                        }
                        // Otherwise, some other genuine error occurred.
                        else
                        {
                            throw new Exception($"Some error has occurred for session: {sessionDto.CircuitShortName}-{sessionDto.SessionKey}.");
                        }
                    }

                    if (lapDtos is null)
                    {
                        throw new Exception("Failed to deserialize lap data.");
                    }

                    // Send the laps into InfluxDB.
                    foreach (RaceLapDto lapDto in lapDtos)
                    {
                        // Do not count null laps or laps with a lap time of 0 or laps without a proper timestamp.
                        if (lapDto.LapDuration is null || lapDto.LapDuration <= 0.0f || lapDto.Date is null)
                        {
                            continue;
                        }

                        await repository.SaveAsync(CreateTelemetryEntry(lapDto));
                    }

                    await Task.Delay(2000);
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

            services.AddSingleton<IInfluxDBClient>(_ =>
            {
                return new InfluxDBClient(
                    "http://localhost:8086",
                    "super-secret-token");
            });

            services.AddSingleton<Formula1Repository>();
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
    }
}
