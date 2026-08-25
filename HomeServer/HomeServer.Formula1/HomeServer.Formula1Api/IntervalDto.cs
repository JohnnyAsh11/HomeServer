using System.Text.Json.Serialization;

namespace HomeServer.Formula1Api
{
    public class IntervalDto
    {

        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("driver_number")]
        public int? DriverNumber { get; set; }

        [JsonPropertyName("gap_to_leader")]
        public float? GapToLeader { get; set; }

        [JsonPropertyName("interval")]
        public float? Interval { get; set; }

        [JsonPropertyName("meeting_key")]
        public int MeetingKey { get; set; }

        [JsonPropertyName("session_key")]
        public int SessionKey { get; set; }

    }
}
