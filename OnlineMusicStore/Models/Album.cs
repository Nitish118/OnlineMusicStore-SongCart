using System.ComponentModel.DataAnnotations;

namespace OnlineMusicStore.Models
{
    public class Album
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Artist { get; set; }

        [Required]
        public string Genre { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        public string CoverImageUrl { get; set; } // Path to image
    }
}
