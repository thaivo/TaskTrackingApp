namespace TaskTrackingApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeveloperAssignment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Assignments", "DevID", c => c.Int(nullable: false));
            CreateIndex("dbo.Assignments", "DevID");
            AddForeignKey("dbo.Assignments", "DevID", "dbo.Developers", "DevID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Assignments", "DevID", "dbo.Developers");
            DropIndex("dbo.Assignments", new[] { "DevID" });
            DropColumn("dbo.Assignments", "DevID");
        }
    }
}
