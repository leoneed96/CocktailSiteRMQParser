using AngleSharp.Parser.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace OxfordParser.Services
{
    public class WordUsagesSelector
    {
        private static readonly Regex _sentenceRegex = new("^[A-Z].+[.]", RegexOptions.Compiled | RegexOptions.Multiline);

        public static List<string> SelectUsages(IEnumerable<string> allUsages, int count)
        {
            var sentenceUsages = allUsages.Where(x => _sentenceRegex.IsMatch(x)).Take(count).ToList();
            if (sentenceUsages.Count >= count)
                return sentenceUsages;

            var restUsages = allUsages.Except(sentenceUsages);

            var misssingCount = count - sentenceUsages.Count;

            return sentenceUsages.Concat(restUsages.Take(misssingCount)).ToList();
        }

        public static List<WordUsageSelectionResult> SelectUsagesFromDetails(List<WordUsageDto> allUsages, int count)
        {
            return allUsages.GroupBy(x => x.CategoryName).Select(x => new WordUsageSelectionResult()
            {
                CategoryName = x.Key,
                SelectedUsages = SelectUsages(x.Select(x => x.Text), count)
            }).ToList();
        }
    }

    public class WordUsageSelectionResult
    {
        public string CategoryName { get; set; }

        public List<string> SelectedUsages { get; set; }
    }
}
