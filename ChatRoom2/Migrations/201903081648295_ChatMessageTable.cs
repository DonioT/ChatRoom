namespace ChatRoom2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChatMessageTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Chat_Messages",
                c => new
                    {
                        ChatMessageId = c.Guid(nullable: false),
                        ChatMessage = c.String(),
                        DateSent = c.DateTime(nullable: false),
                        User_UserId = c.Guid(),
                    })
                .PrimaryKey(t => t.ChatMessageId)
                .ForeignKey("dbo.Chat_Users", t => t.User_UserId)
                .Index(t => t.User_UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Chat_Messages", "User_UserId", "dbo.Chat_Users");
            DropIndex("dbo.Chat_Messages", new[] { "User_UserId" });
            DropTable("dbo.Chat_Messages");
        }
    }
}
