using HomeServer.Formula1Database.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeServer.Formula1Database
{
    /// <summary>
    /// DbContext for the Formula 1 Relational Database.
    /// </summary>
    public class Formula1Context : DbContext
    {
        /// <summary>
        /// The drivers of Formula 1.
        /// </summary>
        public DbSet<F1DriverModel> Drivers { get; set; }

        /// <summary>
        /// The race weekends within Formula 1.
        /// </summary>
        public DbSet<MeetingModel> Meetings { get; set; }

        /// <summary>
        /// The sessions within a race weekend.
        /// </summary>
        public DbSet<SessionModel> Sessions { get; set; }

        /// <summary>
        /// The results of a driver's session.
        /// </summary>
        public DbSet<SessionResultModel> SessionResults { get; set; }
        
        /// <summary>
        /// The laps completed by drivers during sessions.
        /// </summary>
        public DbSet<LapModel> Laps { get; set; }

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<F1DriverModel>()
                .HasIndex(d => d.Id)
                .IsUnique();

            modelBuilder.Entity<MeetingModel> ()
                .HasIndex(m => m.Id)
                .IsUnique();

            modelBuilder.Entity<LapModel>()
                .HasIndex(l => l.Id)
                .IsUnique();

            modelBuilder.Entity<SessionResultModel>()
                .HasIndex(s => s.Id)
                .IsUnique();

            modelBuilder.Entity<SessionModel>()
                .HasIndex(s => s.Id)
                .IsUnique();
        }

        /// <inheritdoc/>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            // Database file within the container.
            // optionsBuilder.UseSqlite("Data Source=/data/Formula1Data.db;");
            optionsBuilder.UseSqlite("Data Source=../../HomeServer.Data/RelationalF1Data/Formula1Data.db;");
        }
    }
}
