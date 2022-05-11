using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using OxfordParser.Data.Entities;

namespace OxfordParser.Data
{
    public class WordsDbContext: DbContext
    {
        public WordsDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Word> Words { get; set; }
        public DbSet<WordCategory> WordCategories { get; set; }
        public DbSet<WordUsage> WordUsages { get; set; }
        public DbSet<WordType> WordTypes { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Word>().HasMany(x => x.Categories).WithMany(x => x.Words)
                .UsingEntity(x => x.ToTable("WordAndWordCategory"));

            modelBuilder.Entity<WordCategory>();
            modelBuilder.Entity<WordUsage>();
            modelBuilder.Entity<WordType>();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.ConfigureWarnings(x => x.Log((RelationalEventId.CommandExecuted, LogLevel.Debug),
                (RelationalEventId.CommandExecuting, LogLevel.Debug)));
        }
    }
}
