using System;
using System.ComponentModel.DataAnnotations;
using WebGrease.Css;

namespace OnlineMusicStore.Models
{
    public class MusicItem
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Artist { get; set; }

        public string Album { get; set; }

        public string Category { get; set; } 

        public DateTime ReleaseDate { get; set; }

        public decimal Price { get; set; }

        public string ImageUrl { get; set; }

        public int VoteCount { get; set; } = 0; 
    }
}