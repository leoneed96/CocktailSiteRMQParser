using AngleSharp.Parser.Models;
using Microsoft.EntityFrameworkCore;
using OxfordParser.Data;
using OxfordParser.Data.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace OxfordParser
{
    public class WordsRepository
    {
        public static Task<bool> ExistsAsync(WordsDbContext dbContext, WordListItem listItem) => 
            dbContext.Words.AsNoTracking().AnyAsync(x => 
                   x.Text == listItem.WordText && 
                   x.WordType.NameEng == listItem.Type);

        public static async Task<WordType> GetOrAddWordTypeAsync(WordsDbContext dbContext, string wordTypeEng)
        {
            var existing = await dbContext.WordTypes.FirstOrDefaultAsync(x => x.NameEng == wordTypeEng);
            if (existing is not null)
                return existing;

            var newType = new WordType()
            {
                NameEng = wordTypeEng
            };

            dbContext.WordTypes.Add(newType);

            return newType;
        }

        public static async Task<Word> CreateWordAsync(WordsDbContext dbContext, WordListItem item, WordDetails itemDetails, WordType wordType, string ukPath, string usPath, System.Collections.Generic.List<Services.WordUsageSelectionResult> selectedUsages)
        {
            var word = new Word()
            {
                Text = item.WordText,
                WordLevel = item.WordLevel,
                WordType = wordType,
                SoundPathUS = usPath,
                SoundPathUK = ukPath
            };

            dbContext.Words.Add(word);
            dbContext.SaveChanges();

            foreach (var category in itemDetails.Categories)
            {
                var dbCategory = await dbContext.WordCategories.FirstOrDefaultAsync(x => x.NameEng == category);
                if(dbCategory is null)
                {
                    dbCategory = new WordCategory()
                    {
                        NameEng = category
                    };

                    dbContext.WordCategories.Add(dbCategory);
                    dbContext.SaveChanges();
                }

                word.Categories.Add(dbCategory);
            }

            foreach (var usage in selectedUsages)
            {
                WordCategory usageCategory = null;

                if(usage.CategoryName is not null)
                {
                    usageCategory = await dbContext.WordCategories.FirstOrDefaultAsync(x => x.NameEng == usage.CategoryName);
                    if (usageCategory is null)
                    {
                        usageCategory = new WordCategory()
                        {
                            NameEng = usage.CategoryName
                        };

                        dbContext.WordCategories.Add(usageCategory);
                        dbContext.SaveChanges();
                    }
                }

                foreach (var usageEntry in usage.SelectedUsages)
                {
                    dbContext.WordUsages.Add(new WordUsage()
                    {
                        Text = usageEntry,
                        WordCategory = usageCategory,
                        Word = word
                    });

                }
                dbContext.SaveChanges();
            }

            dbContext.SaveChanges();
            return word;
        }
    }
}
