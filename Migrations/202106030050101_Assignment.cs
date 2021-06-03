namespace TaskTrackingApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Assignment : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Assignments",
                c => new
                    {
                        AssignmentID = c.Int(nullable: false, identity: true),
                        AssignmentDesc = c.String(),
                        Status = c.Int(nullable: false),
                        Priority = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AssignmentID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Assignments");
        }
    }
}
