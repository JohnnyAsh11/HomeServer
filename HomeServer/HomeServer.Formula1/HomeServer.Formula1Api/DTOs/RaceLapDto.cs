using System.Text.Json.Serialization;

namespace HomeServer.Formula1Api
{
    public class RaceLapDto
    {
        [JsonPropertyName("date_start")]
        public DateTime Date { get; set; }

        [JsonPropertyName("driver_number")]
        public int DriverNumber { get; set; } = 0;

        [JsonPropertyName("duration_sector_1")]
        public float? DurationSector1 { get; set; }

        [JsonPropertyName("duration_sector_2")]
        public float? DurationSector2 { get; set; }

        [JsonPropertyName("duration_sector_3")]
        public float? DurationSector3 { get; set; }

        [JsonPropertyName("i1_speed")]
        public int? I1Speed { get; set; }

        [JsonPropertyName("i2_speed")]
        public int? I2Speed { get; set; }

        [JsonPropertyName("is_pit_out_lap")]
        public bool IsPitOutLap { get; set; } = false;

        [JsonPropertyName("lap_duration")]
        public float? LapDuration { get; set; }

        [JsonPropertyName("lap_number")]
        public int LapNumber { get; set; } = 0;

        [JsonPropertyName("meeting_key")]
        public int MeetingKey { get; set; } = 0;

        [JsonPropertyName("segments_sector_1")]
        public List<int?> MiniSectors1 { get; set; } = [];

        [JsonPropertyName("segments_sector_2")]
        public List<int?> MiniSectors2 { get; set; } = [];

        [JsonPropertyName("segments_sector_3")]
        public List<int?> MiniSectors3 { get; set; } = [];

        [JsonPropertyName("session_key")]
        public int SessionKey { get; set; } = 0;

        [JsonPropertyName("st_speed")]
        public int? SpeedTrap { get; set; }

    }
}
