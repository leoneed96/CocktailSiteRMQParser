using OxfordParser.Data.Entities;
using System.Collections.Generic;

namespace AngleSharp.Parser.Models
{
    public class WordDetails
    {
        public string Text { get; set; }
        /// <summary>
        /// Тип - noun, objective...
        /// </summary>
        public string WordType { get; set; }

        public List<WordUsageDto> Usages { get; set; } = new List<WordUsageDto>();

        public WordLevel WordLevel { get; set; }

        public SpecialWordList SpecialWordList { get; set; }

        public List<string> Categories { get; set; } = new List<string>();
    }

    public class WordUsageDto
    {
        // Sports: ball and racket sports
        public string? CategoryName { get; set; }
        public string Text { get; set; } = default!;
    }
}
