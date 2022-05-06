using OxfordParser.Data.Entities;
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

    
}
