namespace ChatRoom2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class groupChatFix : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Group_Members",
                c => new
                    {
                        GroupMemberId = c.Guid(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UserId = c.Guid(nullable: false),
                        GroupId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.GroupMemberId)
                .ForeignKey("dbo.Groups", t => t.GroupId, cascadeDelete: true)
                .ForeignKey("dbo.Chat_Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.GroupId);
            
            CreateTable(
                "dbo.Groups",
                c => new
                    {
                        GroupId = c.Guid(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.GroupId);
            
            CreateTable(
                "dbo.Group_Messages",
                c => new
                    {
                        GroupMessageId = c.Guid(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        GroupId = c.Guid(nullable: false),
                        ChatMessageId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.GroupMessageId)
                .ForeignKey("dbo.Chat_Messages", t => t.ChatMessageId, cascadeDelete: true)
                .ForeignKey("dbo.Groups", t => t.GroupId, cascadeDelete: true)
                .Index(t => t.GroupId)
                .Index(t => t.ChatMessageId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Group_Messages", "GroupId", "dbo.Groups");
            DropForeignKey("dbo.Group_Messages", "ChatMessageId", "dbo.Chat_Messages");
            DropForeignKey("dbo.Group_Members", "UserId", "dbo.Chat_Users");
            DropForeignKey("dbo.Group_Members", "GroupId", "dbo.Groups");
            DropIndex("dbo.Group_Messages", new[] { "ChatMessageId" });
            DropIndex("dbo.Group_Messages", new[] { "GroupId" });
            DropIndex("dbo.Group_Members", new[] { "GroupId" });
            DropIndex("dbo.Group_Members", new[] { "UserId" });
            DropTable("dbo.Group_Messages");
            DropTable("dbo.Groups");
            DropTable("dbo.Group_Members");
        }
    }
}
