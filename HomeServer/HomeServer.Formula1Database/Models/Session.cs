using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeServer.Database
{
    /// <summary>
    /// Represents a Formula 1 session in the database.
    /// </summary>
    public class Session
    {

        /// <summary>
        /// Unique ID of the task within the database.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Unique ID of Formula 1 session. (Provided by OpenF1)
        /// </summary>
        public int MeetingKey { get; set; }

        /// <summary>
        /// Year that the event took place.
        /// </summary>
        public int? Year { get; set; }

        /// <summary>
        /// The unique key for the circuit. (Provided by OpenF1)
        /// </summary>
        public int? CircuitKey { get; set; }

        /// <summary>
        /// The shortened name of the circuit.
        /// </summary>
        public string? ShortName { get; set; }
        
        /// <summary>
        /// The name of the country hosting the session.
        /// </summary>
        public string? CountryName { get; set; }

        /// <summary>
        /// The name of the session.  (Practice 1/Practice 2/Practice 3...)
        /// </summary>
        public string? SessionName { get; set; }

        /// <summary>
        /// The type of the session.  (Practice/Qualifying/Race)
        /// </summary>
        public string? SessionType { get; set; }

    }
}
