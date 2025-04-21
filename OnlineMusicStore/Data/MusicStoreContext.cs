using System.Data.Entity;
using OnlineMusicStore.Models;

namespace OnlineMusicStore.Data
{
    public class MusicStoreContext : DbContext
    {
        public MusicStoreContext() : base("MusicStoreConnection")
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<MusicItem> MusicItems { get; set; }
        public DbSet<CartItem> CartItem { get; set; }
        public DbSet<WishlistItem> WishlistItems { get; set; } 
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }

    }
}
