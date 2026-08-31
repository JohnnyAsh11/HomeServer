using HomeServer.Formula1Database.Models;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Core.Flux.Domain;
using InfluxDB.Client.Writes;

namespace HomeServer.Formula1Database
{
    /// <summary>
    /// Db context for the Formula1 Influx database.
    /// </summary>
    public class Formula1Repository
    {
        private IInfluxDBClient _client;
        private const string Bucket = "Formula1";
        private const string Org = "HomeServer";

        /// <summary>
        /// Constructs the repository for the InfluxDb database.
        /// </summary>
        /// <param name="client">Injected client for Influx access.</param>
        public Formula1Repository(IInfluxDBClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Saves a telemetry entry to the Influx database.
        /// </summary>
        public async Task SaveAsync(TelemetryEntry telemetry)
        {
            IWriteApiAsync write = _client.GetWriteApiAsync();

            // NOTE:
            // The Driver Number, Session and Meeting are tagged.
            // That is because these parts of the entry should act as
            // headers for the sector and lap timing data.
            PointData point = PointData
                .Measurement("telemetry")
                .Tag("driver number", telemetry.DriverNumber.ToString())
                .Field("Sector 1", telemetry.Sector1)
                .Field("Sector 2", telemetry.Sector2)
                .Field("Sector 3", telemetry.Sector3)
                .Field("Lap Time", telemetry.LapTime)
                .Field("Lap Number", telemetry.LapNumber)
                .Tag("session", telemetry.SessionKey.ToString())
                .Tag("meeting", telemetry.MeetingKey.ToString())
                .Timestamp(telemetry.Timestamp, WritePrecision.Ms);

            await write.WritePointAsync(point, Bucket, Org);
        }

        public async Task QueryAsync()
        {
            IQueryApi queryApi = _client.GetQueryApi();

            DateTime start = DateTime.Now;
            DateTime end = start.AddDays(-7);
            string query =
                $"""
                from(bucket: "Formula1")
                    |> range(start: {end:yyyy-MM-ddTHH:mm:ssZ}, stop: {start:yyyy-MM-ddTHH:mm:ssZ})
                    |> filter(fn: (r) => r["_measurement"] == "telemetry")
                    |> filter(fn: (r) => r["_field"] == "Lap Time")
                """;

            List<FluxTable> tables = await queryApi.QueryAsync(query, Org);

            foreach (FluxTable table in tables)
            {
                foreach (FluxRecord? record in table.Records)
                {
                    if (record is null)
                    {
                        continue;
                    }

                    Console.WriteLine($"{record.GetTime()}: {record.GetValue()}");
                }
            }
        }
    }
}
