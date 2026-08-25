using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeServer.Formula1Database.Models
{
    public class TelemetryEntry
    {
        public DateTime Timestamp { get; set; }

        public int DriverNumber { get; set; }

        public float Sector1 { get; set; }

        public float Sector2 { get; set; }

        public float Sector3 { get; set; }

        public float LapTime { get; set; }

        public int LapNumber { get; set; }

        public int MeetingKey { get; set; }

        public int SessionKey { get; set; }
    }
}
