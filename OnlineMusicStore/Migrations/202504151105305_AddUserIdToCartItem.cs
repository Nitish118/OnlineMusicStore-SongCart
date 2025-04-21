namespace OnlineMusicStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserIdToCartItem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CartItems", "UserId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CartItems", "UserId");
        }
    }
}
