using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HomeServer.Formula1Database
{
    /// <summary>
    /// Defines a race weekend in Formula 1.
    /// </summary>
    public class MeetingModel
    {

        /// <summary>
        /// The unique ID of this meeting database entry.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The year this session took place in.
        /// </summary>
        public int? Year { get; set; }

        /// <summary>
        /// The location this race took place in.
        /// </summary>
        public string Location { get; set; } = string.Empty;

        /// <summary>
        /// The official name of the race weekend.
        /// </summary>
        public string MeetingName { get; set; } = string.Empty;

    }
}
