using System;
using System.Collections.Generic;
using System.Text;

namespace HomeServer.Formula1Database.Models
{
    public class TelemetryEntry
    {
        public DateTime Timestamp { get; set; }

        public int DriverNumber { get; set; }

        public string Driver { get; set; } = "";

        public int Lap { get; set; }

        public double Speed { get; set; }

        public int Gear { get; set; }

        public double Throttle { get; set; }

        public double Brake { get; set; }

        public double Rpm { get; set; }

        public double DRS { get; set; }

        public string Session { get; set; } = "";
    }
}
