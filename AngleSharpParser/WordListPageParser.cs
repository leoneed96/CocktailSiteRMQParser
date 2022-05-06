using AngleSharp.Html.Parser;
using AngleSharp.Parser.Models;
using System.Collections.Generic;

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

                entry.DetailsLink = item.QuerySelector("a").GetAttribute("href");
                entry.Type = item.QuerySelector(".pos").TextContent;
                entry.AmericanSoundLink = item.QuerySelector(".pron-us").GetAttribute("data-src-mp3");
                entry.UKSoundLink = item.QuerySelector(".pron-uk").GetAttribute("data-src-mp3");

                yield return entry;
            }
        }
    }
}
