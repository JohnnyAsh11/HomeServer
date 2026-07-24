using HomeServer.Formula1Database.Models;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;

namespace HomeServer.Formula1Database
{
    /// <summary>
    /// Db context for the Formula1 database.
    /// </summary>
    public class Formula1Repository
    {
        private IInfluxDBClient _client;
        private const string Bucket = "Formula1";
        private const string Org = "HomeServer";

        public Formula1Repository(IInfluxDBClient client)
        {
            _client = client;
        }

        public async Task SaveAsync(TelemetryEntry telemetry)
        {
            IWriteApiAsync write = _client.GetWriteApiAsync();
            PointData point = PointData
                .Measurement("telemetry")
                .Tag("driver", telemetry.Driver)
                .Tag("session", telemetry.Session)
                .Field("speed", telemetry.Speed)
                .Field("gear", telemetry.Gear)
                .Field("throttle", telemetry.Throttle)
                .Field("brake", telemetry.Brake)
                .Field("rpm", telemetry.Rpm)
                .Field("drs", telemetry.DRS)
                .Field("lap", telemetry.Lap)
                .Timestamp(telemetry.Timestamp, WritePrecision.Ms);

            await write.WritePointAsync(point, Bucket, Org);
        }
    }
}
