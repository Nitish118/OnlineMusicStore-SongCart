namespace OnlineMusicStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddWishListItem : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.WishlistItems", "MusicItemId", "dbo.MusicItems");
            DropForeignKey("dbo.WishlistItems", "UserId", "dbo.Users");
            DropIndex("dbo.WishlistItems", new[] { "UserId" });
            DropIndex("dbo.WishlistItems", new[] { "MusicItemId" });
            AddColumn("dbo.WishlistItems", "SongId", c => c.Int(nullable: false));
            AddColumn("dbo.WishlistItems", "Title", c => c.String());
            AddColumn("dbo.WishlistItems", "Artist", c => c.String());
            AddColumn("dbo.WishlistItems", "CoverImageUrl", c => c.String());
            AddColumn("dbo.WishlistItems", "Price", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.WishlistItems", "UserId");
            DropColumn("dbo.WishlistItems", "MusicItemId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.WishlistItems", "MusicItemId", c => c.Int(nullable: false));
            AddColumn("dbo.WishlistItems", "UserId", c => c.Int(nullable: false));
            DropColumn("dbo.WishlistItems", "Price");
            DropColumn("dbo.WishlistItems", "CoverImageUrl");
            DropColumn("dbo.WishlistItems", "Artist");
            DropColumn("dbo.WishlistItems", "Title");
            DropColumn("dbo.WishlistItems", "SongId");
            CreateIndex("dbo.WishlistItems", "MusicItemId");
            CreateIndex("dbo.WishlistItems", "UserId");
            AddForeignKey("dbo.WishlistItems", "UserId", "dbo.Users", "Id", cascadeDelete: true);
            AddForeignKey("dbo.WishlistItems", "MusicItemId", "dbo.MusicItems", "Id", cascadeDelete: true);
        }
    }
}
