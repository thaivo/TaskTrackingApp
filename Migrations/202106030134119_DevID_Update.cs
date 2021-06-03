namespace TaskTrackingApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DevID_Update : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Assignments", "DevID", "dbo.Developers");
            DropIndex("dbo.Assignments", new[] { "DevID" });
            AlterColumn("dbo.Assignments", "DevID", c => c.Int());
            CreateIndex("dbo.Assignments", "DevID");
            AddForeignKey("dbo.Assignments", "DevID", "dbo.Developers", "DevID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Assignments", "DevID", "dbo.Developers");
            DropIndex("dbo.Assignments", new[] { "DevID" });
            AlterColumn("dbo.Assignments", "DevID", c => c.Int(nullable: false));
            CreateIndex("dbo.Assignments", "DevID");
            AddForeignKey("dbo.Assignments", "DevID", "dbo.Developers", "DevID", cascadeDelete: true);
        }
    }
}
