using System;

namespace OnlineMusicStore.Models
{
    public class CartItem
    {
        public int CartItemId { get; set; } // Primary key
        public int UserId { get; set; } // To associate the cart item with a user
        public int SongId { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string CoverImageUrl { get; set; }
    }
}
