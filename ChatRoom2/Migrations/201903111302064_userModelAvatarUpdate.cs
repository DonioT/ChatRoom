namespace ChatRoom2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userModelAvatarUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Chat_Users", "AvatarUrl", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Chat_Users", "AvatarUrl");
        }
    }
}
