using OxfordParser.Data.Entities;

namespace AngleSharp.Parser.Models
{
    public class WordListItem
    {
        /// <summary>
        /// Тип слова - сущ/прил...
        /// </summary>
        public string Type { get; internal set; }

        /// <summary>
        /// Слово
        /// </summary>
        public string WordText { get; internal set; }

        /// <summary>
        /// Категория слова
        /// </summary>
        public WordLevel WordLevel { get; internal set; }

        /// <summary>
        /// Ссылка на детальную информацию
        /// </summary>
        public string DetailsLink { get; internal set; }

        /// <summary>
        /// Ссылка на озвучку UK
        /// </summary>
        public string UKSoundLink { get; internal set; }

        /// <summary>
        /// Ссылка на озвучку American
        /// </summary>
        public string AmericanSoundLink { get; internal set; }
    }

    
}
