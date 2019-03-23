namespace ChatRoom2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class messageFlags : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Message_Flags",
                c => new
                    {
                        MessageFlagId = c.Guid(nullable: false),
                        MessageSeen = c.Boolean(nullable: false),
                        UserId = c.Guid(nullable: false),
                        ChatMessageId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.MessageFlagId)
                .ForeignKey("dbo.Chat_Messages", t => t.ChatMessageId, cascadeDelete: true)
                .ForeignKey("dbo.Chat_Users", t => t.UserId, cascadeDelete: false)
                .Index(t => t.UserId)
                .Index(t => t.ChatMessageId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Message_Flags", "UserId", "dbo.Chat_Users");
            DropForeignKey("dbo.Message_Flags", "ChatMessageId", "dbo.Chat_Messages");
            DropIndex("dbo.Message_Flags", new[] { "ChatMessageId" });
            DropIndex("dbo.Message_Flags", new[] { "UserId" });
            DropTable("dbo.Message_Flags");
        }
    }
}
