using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OxfordParser.Data.Entities
{
    public class WordCategory
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(128)]
        [Required]
        public string NameEng { get; set; }

        [MaxLength(128)]
        public string NameRu { get; set; }

        [MaxLength(512)]
        public string PicturePath { get; set; }

        public ICollection<Word> Words { get; set; }


    }
}
