using AngleSharp.Html.Parser;
using AngleSharp.Parser.Models;
using Microsoft.Extensions.Logging;
using OxfordParser.Data.Entities;
using System;
using System.Collections.Generic;
using System.Threading;

namespace AngleSharp.Parser
{
    public class WordListPageParser
    {
        private readonly ILogger<WordListPageParser> _logger;
        public WordListPageParser(ILogger<WordListPageParser> logger)
        {
            _logger = logger;
        }

        public IEnumerable<WordListItem> GetWordListItems(string html, CancellationToken ct)
        {
            var parser = new HtmlParser();
            var document = parser.ParseDocument(html);
            var words = document.QuerySelectorAll("li[data-hw]");
            foreach (var item in words)
            {
                
                if (ct.IsCancellationRequested)
                {
                    _logger.LogInformation("Cancellation requested. Stopping list parsing");
                    break;
                }

                var entry = new WordListItem();
                try
                {
                    entry.WordText = item.GetAttribute("data-hw");

                    if (string.IsNullOrEmpty(entry.WordText))
                    {
                        _logger.LogError("Word text is not found for html part {0}", item.OuterHtml);
                        continue;
                    }

                    var wordLevelElement = item.QuerySelector("span.belong-to");
                    if (wordLevelElement is null)
                    {
                        _logger.LogError("Word level dom element is not found for word '{0}', element supposed to be hidden", entry.WordText);
                        continue;
                    }

                    entry.DetailsLink = item.QuerySelector("a").GetAttribute("href");
                    entry.Type = item.QuerySelector(".pos").TextContent;
                    entry.AmericanSoundLink = item.QuerySelector(".pron-us")?.GetAttribute("data-src-mp3");
                    entry.UKSoundLink = item.QuerySelector(".pron-uk")?.GetAttribute("data-src-mp3");
                    if (!Enum.TryParse<WordLevel>(wordLevelElement.TextContent, out var level))
                    {
                        _logger.LogError("Ошибка разбора уровня {0}. Html: {1}", entry.WordText, item.OuterHtml);
                    }
                    entry.WordLevel = level;
                }
                catch(Exception ex)
                {
                    _logger.LogCritical(ex, "Unhandler error while processing html {0}", item.OuterHtml);
                    continue;
                }
                yield return entry;
            }
        }
    }
}
