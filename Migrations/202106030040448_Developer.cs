namespace TaskTrackingApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Developer : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Developers",
                c => new
                    {
                        DevID = c.Int(nullable: false, identity: true),
                        fname = c.String(),
                        lname = c.String(),
                        position = c.String(),
                    })
                .PrimaryKey(t => t.DevID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Developers");
        }
    }
}
