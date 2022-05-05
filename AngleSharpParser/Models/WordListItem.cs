using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngleSharp.Parser.Models
{
    public class WordListItem
    {
        /// <summary>
        /// Тип слова - сущ/прил...
        /// </summary>
        public string Type { get; internal set; }

        /// <summary>
        /// Категория слова в списке 3000
        /// </summary>
        public WordCategory? Category3000 { get; internal set; }

        /// <summary>
        /// Категория слова в списке 5000
        /// </summary>
        public WordCategory? Category5000 { get; internal set; }

        /// <summary>
        /// Слово
        /// </summary>
        public string WordText { get; internal set; }

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

    public enum WordCategory
    {
        a1,
        a2,
        b1,
        b2,
        c1,
        c2
    }
}
