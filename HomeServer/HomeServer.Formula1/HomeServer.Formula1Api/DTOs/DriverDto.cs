using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HomeServer.Formula1Api
{
    public class DriverDto
    {
        [JsonPropertyName("broadcast_name")]
        public string? BroadcastName { get; set; }

        [JsonPropertyName("driver_number")]
        public int? DriverNumber { get; set; }

        [JsonPropertyName("first_name")]
        public string? FirstName { get; set; }

        [JsonPropertyName("full_name")]
        public string? FullName { get; set; }

        [JsonPropertyName("headshot_url")]
        public string? HeadshotUrl { get; set; }

        [JsonPropertyName("last_name")]
        public string? LastName { get; set; }

        [JsonPropertyName("meeting_key")]
        public int? MeetingKey { get; set; }

        [JsonPropertyName("name_acronym")]
        public string? NameAcronym { get; set; }

        [JsonPropertyName("session_key")]
        public int? SessionKey { get; set; }

        [JsonPropertyName("team_colour")]
        public string? TeamColor { get; set; }

        [JsonPropertyName("team_name")]
        public string? TeamName { get; set; }
    }
}