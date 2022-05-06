using System.ComponentModel.DataAnnotations;

namespace OxfordParser.Data.Entities
{
    public class WordUsage
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(1024)]
        [Required]
        public string Text { get; set; }

        [MaxLength(1024)]
        public string Comment { get; set; }

        public Word Word { get; set; }

        public int WordId { get; set; }
    }
}
