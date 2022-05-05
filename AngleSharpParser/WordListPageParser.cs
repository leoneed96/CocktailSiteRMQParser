using AngleSharp.Html.Parser;
using AngleSharp.Parser.Models;
using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngleSharp.Parser
{
    public class WordListPageParser
    {
        public WordListPageParser()
        {
        }

        public IEnumerable<WordListItem> GetWordListItems(string html)
        {
            var parser = new HtmlParser();
            var document = parser.ParseDocument(html);
            var words = document.QuerySelectorAll("li[data-hw]");
            foreach (var item in words)
            {
                var entry = new WordListItem();
                entry.WordText = item.GetAttribute("data-hw");
                if (string.IsNullOrEmpty(entry.WordText))
                    continue;

                entry.Category3000 = ParseCategory(item.GetAttribute("data-ox3000"));    
                entry.Category5000 = ParseCategory(item.GetAttribute("data-ox5000"));
                entry.DetailsLink = item.QuerySelector("a").GetAttribute("href");
                entry.Type = item.QuerySelector(".pos").TextContent;
                entry.AmericanSoundLink = item.QuerySelector(".pron-us").GetAttribute("data-src-mp3");
                entry.UKSoundLink = item.QuerySelector(".pron-uk").GetAttribute("data-src-mp3");

                yield return entry;
            }
        }

        private WordCategory? ParseCategory(string attributeValue)
        {
            if (string.IsNullOrEmpty(attributeValue))
                return null;

            return Enum.TryParse<WordCategory>(attributeValue, out var category) ? category : null;
        }
    }
}
