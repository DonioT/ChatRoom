namespace ChatRoom2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateChatMessageTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Chat_Messages", "User_UserId", "dbo.Chat_Users");
            DropIndex("dbo.Chat_Messages", new[] { "User_UserId" });
            RenameColumn(table: "dbo.Chat_Messages", name: "User_UserId", newName: "UserId");
            AlterColumn("dbo.Chat_Messages", "UserId", c => c.Guid(nullable: false));
            CreateIndex("dbo.Chat_Messages", "UserId");
            AddForeignKey("dbo.Chat_Messages", "UserId", "dbo.Chat_Users", "UserId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Chat_Messages", "UserId", "dbo.Chat_Users");
            DropIndex("dbo.Chat_Messages", new[] { "UserId" });
            AlterColumn("dbo.Chat_Messages", "UserId", c => c.Guid());
            RenameColumn(table: "dbo.Chat_Messages", name: "UserId", newName: "User_UserId");
            CreateIndex("dbo.Chat_Messages", "User_UserId");
            AddForeignKey("dbo.Chat_Messages", "User_UserId", "dbo.Chat_Users", "UserId");
        }
    }
}
