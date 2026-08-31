using System.Text.Json.Serialization;

namespace HomeServer.Formula1Api.DTOs
{
    /// <summary>
    /// Describes the overall race weekend.
    /// </summary>
    public class MeetingDto
    {
        [JsonPropertyName("circuit_key")]
        public int? CircuitKey { get; set; }

        [JsonPropertyName("circuit_info_url")]
        public string? CircuitInfoUrl { get; set; }

        [JsonPropertyName("circuit_image")]
        public string? CircuitImage { get; set; }

        [JsonPropertyName("circuit_short_name")]
        public string? CircuitShortName { get; set; }

        [JsonPropertyName("circuit_type")]
        public string? CircuitType { get; set; }

        [JsonPropertyName("country_code")]  
        public string? CountryCode { get; set; }

        [JsonPropertyName("country_flag")]
        public string? CountryFlag { get; set; }

        [JsonPropertyName("country_key")]
        public int? CountryKey { get; set; }

        [JsonPropertyName("country_name")]
        public string? CountryName { get; set; }

        [JsonPropertyName("date_end")]
        public DateTime? DateEnd { get; set; }

        [JsonPropertyName("date_start")]
        public DateTime? DateStart { get; set; }

        [JsonPropertyName("is_cancelled")]
        public bool? IsCancelled { get; set; }

        [JsonPropertyName("location")]
        public string? Location { get; set; }

        [JsonPropertyName("meeting_key")]
        public int? MeetingKey { get; set; }

        [JsonPropertyName("meeting_official_name")]
        public string? MeetingOfficialName { get; set; }

        [JsonPropertyName("meeting_name")]
        public string? MeetingName { get; set; }

        //[JsonPropertyName("year")]
        //public int? Year { get; set; }
    }
}
