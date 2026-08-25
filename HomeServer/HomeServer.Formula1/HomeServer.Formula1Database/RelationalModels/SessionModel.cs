using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeServer.Formula1Database.Models
{
    /// <summary>
    /// Represents a Formula 1 session in the database.
    /// </summary>
    public class SessionModel
    {

        /// <summary>
        /// Unique ID of the task within the database.
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// The database generated ID of the parent meeting.
        /// </summary>
        public int MeetingId { get; set; }


        /// <summary>
        /// Unique ID of Formula 1 session. (Provided by OpenF1)
        /// </summary>
        public int MeetingKey { get; set; }
        /// <summary>
        /// The unique key for the circuit. (Provided by OpenF1)
        /// </summary>
        public int? CircuitKey { get; set; }


        /// <summary>
        /// The name of the session.  (Practice 1/Practice 2/Practice 3...)
        /// </summary>
        public string? SessionName { get; set; }

        /// <summary>
        /// The type of the session.  (Practice/Qualifying/Race)
        /// </summary>
        public string? SessionType { get; set; }

        /// <summary>
        /// The temperature of the track at the beginning of the session.
        /// </summary>
        public int? TrackTemp { get; set; }

        /// <summary>
        /// Weather or not rain conditions were present during the session.
        /// </summary>
        public bool Rainfall { get; set; }

        /// <summary>
        /// The air pressure at the track (mbar).
        /// </summary>
        public int AirPressure { get; set; }

    }
}
