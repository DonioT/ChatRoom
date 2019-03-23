namespace ChatRoom2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class groupchattableupdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Groups", "LastModified", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Groups", "LastModified");
        }
    }
}
