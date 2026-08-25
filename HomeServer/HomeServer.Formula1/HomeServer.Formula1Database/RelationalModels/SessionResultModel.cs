using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeServer.Formula1Database.Models
{
    /// <summary>
    /// The individual session result of a driver.
    /// </summary>
    public class SessionResultModel
    {
        /// <summary>
        /// Unique ID of the task within the database.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Database generated unique ID of the session.
        /// </summary>
        public int SessionId { get; set; }

        /// <summary>
        /// Database generated unique ID of the meeting.
        /// </summary>
        public int MeetingId { get; set; }

        /// <summary>
        /// Unique ID of Formula 1 meeting. (Provided by OpenF1)
        /// </summary>
        public int MeetingKey { get; set; }

        /// <summary>
        /// Unique ID of Formula 1 session. (Provided by OpenF1)
        /// </summary>
        public int SessionKey { get; set; }

        /// <summary>
        /// Did not finish?
        /// </summary>
        public bool? DNF { get; set; }

        /// <summary>
        /// Did not start?
        /// </summary>
        public bool? DNS { get; set; }

        /// <summary>
        /// Got disqualified?
        /// </summary>
        public bool? DSQ { get; set; }

        /// <summary>
        /// Driver associated with this SessionResult.
        /// </summary>
        public int? DriverNumber { get; set; }

        /// <summary>
        /// Finishing position of the driver in this session.
        /// </summary>
        public int? FinishingPosition { get; set; }

        /// <summary>
        /// The time (in seconds) from the leader/winner of this session.
        /// </summary>
        public float GapToLeader { get; set; }

        /// <summary>
        /// The number of pitstops the driver made during this session.
        /// </summary>
        public int? NumberOfPitstops { get; set; }

        /// <summary>
        /// The seqeunce of tire compounds used by the driver.
        /// </summary>
        public string Tires { get; set; } = string.Empty;
    }
}
