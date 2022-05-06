using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OxfordParser.Data.Entities
{
    public class Word
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(512)]
        [Required]
        public string Text { get; set; }

        [MaxLength(512)]
        [Required]
        public string SoundPathUK { get; set; }

        [MaxLength(512)]
        [Required]
        public string SoundPathUS { get; set; }

        public WordLevel WordLevel { get; set; }

        [MaxLength(512)]
        public string PicturePath { get; set; }

        public WordType WordType { get; set; }

        public int WordTypeId { get; set; }

        public ICollection<WordUsage> Usages { get; set; } = new List<WordUsage>();

        public ICollection<WordCategory> Categories { get; set; }
        public bool Processed { get; set; }
    }
}
