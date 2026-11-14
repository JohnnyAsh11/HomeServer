using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace HomeServer.Database
{
    public class HomeServerContext : DbContext
    {
        /// <summary>
        /// A collection of task objects that need to be completed.
        /// </summary>
        public DbSet<HomeServerTask> Tasks { get; set; }

        /// <summary>
        /// A collection of all of the chat history between the user and the AI assistant.
        /// </summary>
        public DbSet<StoredChatHistory> ChatHistories { get; set; }

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<HomeServerTask>()
                .HasIndex(t => t.Id)
                .IsUnique();

            modelBuilder.Entity<StoredChatHistory>()
                .HasIndex(h => h.Id)
                .IsUnique();
        }

        /// <inheritdoc/>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            // Database file local to this project. (for debugging purposes)
            // optionsBuilder.UseSqlite("Data Source=../HomeServer.Data/TodoDatabase/TodoData.db;");

            // Database file within the container.
            optionsBuilder.UseSqlite("Data Source=/data/HomeServerData.db;");
        }
    }
}
