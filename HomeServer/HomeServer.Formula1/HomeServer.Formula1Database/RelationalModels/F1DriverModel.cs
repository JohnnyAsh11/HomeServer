using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeServer.Formula1Database.Models
{
    /// <summary>
    /// Represents a Formula 1 driver in the database.
    /// </summary>
    public class F1DriverModel
    {
        /// <summary>
        /// Unique ID of the driver.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The driver's name as it displays on the screen.
        /// </summary>
        public string? BroadcastName { get; set; }

        /// <summary>
        /// The driver's racing number.
        /// </summary>
        public int? DriverNumber { get; set; }

        /// <summary>
        /// The driver's full name.
        /// </summary>
        public string? DriverName { get; set; }
        
        /// <summary>
        /// The driver's name as it would appear on timing charts.
        /// </summary>
        public string? NameAcronym { get; set; }

        /// <summary>
        ///  Link to a headshot of the driver.
        /// </summary>
        public string? HeadshotUrl { get; set; }

        /// <summary>
        /// The name of the team the driver is currently racing for.
        /// </summary>
        public string? TeamName { get; set; }
    }
}
