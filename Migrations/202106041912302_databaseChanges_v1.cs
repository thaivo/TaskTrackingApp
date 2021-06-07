namespace TaskTrackingApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class databaseChanges_v1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Developers", "DeveloperPosition", c => c.String());
            DropColumn("dbo.Developers", "Position");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Developers", "Position", c => c.String());
            DropColumn("dbo.Developers", "DeveloperPosition");
        }
    }
}
