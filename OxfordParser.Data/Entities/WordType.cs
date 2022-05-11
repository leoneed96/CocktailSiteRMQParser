using System.ComponentModel.DataAnnotations;

namespace OxfordParser.Data.Entities
{
    public class WordType
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(128)]
        [Required]
        public string NameEng { get; set; }

        [MaxLength(128)]
        public string NameRu { get; set; }
    }
}
