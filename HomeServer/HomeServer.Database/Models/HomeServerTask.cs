using System.ComponentModel.DataAnnotations;

namespace HomeServer.Database
{
    public class HomeServerTask
    {
        /// <summary>
        /// Unique ID of the task.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The title of the task.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// The description of the task.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// The estimated amount of time for this task to be completed.
        /// </summary>
        public float EstimatedTime { get; set; }

        /// <summary>
        /// Determines when the due date of the task is.
        /// </summary>
        public DateTime DueDate { get; set; }

        /// <summary>
        /// Determines whether or not the task is complete.
        /// </summary>
        public bool? IsComplete { get; set; }

        /// <summary>
        /// The time at which the task was created.
        /// </summary>
        public DateTime StartTime { get; set; } = DateTime.Now;

        /// <summary>
        /// The time at which the task was completed.
        /// </summary>
        public DateTime EndTime { get; set; }
    }
}
