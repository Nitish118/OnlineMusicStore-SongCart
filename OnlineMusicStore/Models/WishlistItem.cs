namespace OnlineMusicStore.Models
{
    public class WishlistItem
    {
        public int Id { get; set; } 
        public int SongId { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string CoverImageUrl { get; set; }
        public decimal Price { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }  
    }
}
