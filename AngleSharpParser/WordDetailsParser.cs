using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using AngleSharp.Parser.Models;
using OxfordParser.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngleSharp.Parser
{
    public class WordDetailsParser
    {
        private readonly HtmlParser _htmlParser = new HtmlParser();
        public async Task<WordDetails> GetWordDetails(string html)
        {
            var doc = await _htmlParser.ParseDocumentAsync(html);
            var document = doc.QuerySelector("#main_column");
            var result = new WordDetails();

            var wordType = document.QuerySelector("span.pos")?.TextContent;
            if (string.IsNullOrEmpty(wordType))
                throw new ArgumentNullException("WordType");
            result.WordType = wordType;

            var headword = document.QuerySelector("h1.headword");

            if (string.IsNullOrEmpty(headword.TextContent))
                throw new ArgumentNullException("WordText");
            result.Text = headword.TextContent;

            // check this
            if (headword.GetAttribute("ox3000") == "y")
                result.SpecialWordList &= SpecialWordList.Oxford3000;
            if (headword.GetAttribute("ox5000") == "y")
                result.SpecialWordList &= SpecialWordList.Oxford5000;

            // Если есть уровень слова вне контекста категории
            SetWordLevelBySymbols(document, result);


            // Мешаются
            var idioms = document.QuerySelector("div.idioms");
            if (idioms is not null)
                idioms.Remove();

            // senses
            foreach (var sense in document.QuerySelectorAll("li.sense"))
            {
                var topic = sense.QuerySelector("span.topic-g")?.QuerySelector("span.topic_name")?.TextContent;

                // Если не нашли уровень по символам, возьмем из первой попавшейся категории
                var level = sense.QuerySelector("span.topic-g")?.QuerySelector("span.topic_cerf")?.TextContent;
                if (result.WordLevel == WordLevel.None && !string.IsNullOrWhiteSpace(level))
                {
                    if (Enum.TryParse<WordLevel>(level.ToLower(), out var enumLevel))
                    {
                        result.WordLevel = enumLevel;
                    }
                }


                var hasCategory = !string.IsNullOrEmpty(topic);

                if (hasCategory)
                {
                    result.Categories.Add(topic);
                }

                var usages = sense.QuerySelector("ul.examples")?.QuerySelectorAll("span.x");
                if (usages is not null)
                    foreach (var usage in usages)
                    {
                        result.Usages.Add(new WordUsageDto()
                        {
                            Text = usage.TextContent,
                            CategoryName = hasCategory ? topic : null
                        });
                    }

            }


            return result;
        }

        private static void SetWordLevelBySymbols(IElement document, WordDetails result)
        {
            foreach (var item in document.QuerySelectorAll("div.symbols"))
            {
                if (item.QuerySelector(".ox3ksym_a1, .ox5ksym_a1") is not null)
                {
                    result.WordLevel = WordLevel.a1;
                    break;
                }
                if (item.QuerySelector(".ox3ksym_a2, .ox5ksym_a2") is not null)
                {
                    result.WordLevel = WordLevel.a2;
                    break;
                }
                if (item.QuerySelector(".ox3ksym_b1, .ox5ksym_b1") is not null)
                {
                    result.WordLevel = WordLevel.b1;
                    break;
                }
                if (item.QuerySelector(".ox3ksym_b2, .ox5ksym_b2") is not null)
                {
                    result.WordLevel = WordLevel.b2;
                    break;
                }
                if (item.QuerySelector(".ox3ksym_c1, .ox5ksym_c1") is not null)
                {
                    result.WordLevel = WordLevel.c1;
                    break;
                }
                if (item.QuerySelector(".ox3ksym_c2, .ox5ksym_c2") is not null)
                {
                    result.WordLevel = WordLevel.c2;
                    break;
                }
            }
        }
    }
}
