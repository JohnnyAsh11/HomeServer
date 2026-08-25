using System.ComponentModel.DataAnnotations;

namespace HomeServer.Formula1Database
{
    public class LapModel
    {

        /// <summary>
        /// The unique ID of this lap entry.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The unique ID of the driver who completed this lap.
        /// </summary>
        public int DriverId { get; set; }
        /// <summary>
        /// The unique ID of the session in which this lap was completed.
        /// </summary>
        public int SessionId { get; set; }

        /// <summary>
        /// The lap number within this session.
        /// </summary>
        public int? LapNumber { get; set; }

        /// <summary>
        /// The tire compound used for this lap.
        /// </summary>
        public string TireCompound { get; set; } = string.Empty;

        /// <summary>
        /// The lap time of this lap.
        /// </summary>
        public float? LapTime { get; set; }

        /// <summary>
        /// The first sector time of this lap.
        /// </summary>
        public float? Sector1 { get; set; }

        /// <summary>
        /// The second sector time of this lap.
        /// </summary>
        public float? Sector2 { get; set; }

        /// <summary>
        /// The third sector time of this lap.
        /// </summary>
        public float? Sector3 { get; set; }

        /// <summary>
        /// The mini sector times for the first sector of this lap.
        /// </summary>
        public List<int?> MiniSectors1 { get; set; } = [];

        /// <summary>
        /// The mini sector times for the second sector of this lap.
        /// </summary>
        public List<int?> MiniSectors2 { get; set; } = [];

        /// <summary>
        /// The mini sector times for the third sector of this lap.
        /// </summary>
        public List<int?> MiniSectors3 { get; set; } = [];

        /// <summary>
        /// The speed trap speed for this lap.
        /// </summary>
        public int? SpeedTrap { get; set; }

    }
}
