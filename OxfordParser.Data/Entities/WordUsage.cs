using System.ComponentModel.DataAnnotations;

namespace OxfordParser.Data.Entities
{
    public class WordUsage
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(512)]
        [Required]
        public string Text { get; set; }

        [MaxLength(512)]
        public string Comment { get; set; }

        public Word Word { get; set; }

        public int WordId { get; set; }

        public WordCategory WordCategory { get; set; }
        public int? WordCategoryId { get; set; }
    }
}
