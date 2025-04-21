namespace OnlineMusicStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateWishlistItemModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WishlistItems", "UserId", c => c.Int(nullable: false));
            CreateIndex("dbo.WishlistItems", "UserId");
            AddForeignKey("dbo.WishlistItems", "UserId", "dbo.Users", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WishlistItems", "UserId", "dbo.Users");
            DropIndex("dbo.WishlistItems", new[] { "UserId" });
            DropColumn("dbo.WishlistItems", "UserId");
        }
    }
}
