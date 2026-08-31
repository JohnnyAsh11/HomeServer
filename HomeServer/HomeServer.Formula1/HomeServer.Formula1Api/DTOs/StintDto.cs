using System.Text.Json.Serialization;

namespace HomeServer.Formula1Api.DTOs
{
    /// <summary>
    /// Contains tire stint data for a given driver.
    /// </summary>
    public class StintDto
    {
        [JsonPropertyName("compound")]
        public string? Compound { get; set; }

        [JsonPropertyName("driver_number")]
        public int? DriverNumber { get; set; }

        [JsonPropertyName("lap_end")]
        public int? LapEnd { get; set; }

        [JsonPropertyName("lap_start")]
        public int? LapStart { get; set; }

        [JsonPropertyName("meeting_key")]
        public int? MeetingKey { get; set; }

        [JsonPropertyName("session_key")]
        public int? SessionKey { get; set; }

        [JsonPropertyName("stint_number")]
        public int? StintNumber { get; set; }

        [JsonPropertyName("tire_age_at_start")]
        public int? TireAgeAtStart { get; set; }

    }
}
