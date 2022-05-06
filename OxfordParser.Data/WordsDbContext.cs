using Microsoft.EntityFrameworkCore;
using OxfordParser.Data.Entities;

namespace OxfordParser.Data
{
    public class WordsDbContext: DbContext
    {
        public WordsDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Word>().HasMany(x => x.Categories).WithMany(x => x.Words)
                .UsingEntity(x => x.ToTable("WordAndWordCategory"));

            modelBuilder.Entity<WordCategory>();
            modelBuilder.Entity<WordUsage>();
            modelBuilder.Entity<WordType>();
        }
    }
}
