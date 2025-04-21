namespace OnlineMusicStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddReplyMessageToFeedback : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Feedbacks", "ReplyMessage", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Feedbacks", "ReplyMessage");
        }
    }
}
