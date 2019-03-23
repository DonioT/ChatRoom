namespace ChatRoom2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GroupChat : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Chat_Messages", "Private", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Chat_Messages", "Private");
        }
    }
}
