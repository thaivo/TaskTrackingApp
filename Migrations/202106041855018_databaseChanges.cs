namespace TaskTrackingApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class databaseChanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Developers", "DeveloperFirstName", c => c.String());
            AddColumn("dbo.Developers", "DeveloperLastName", c => c.String());
            DropColumn("dbo.Developers", "fname");
            DropColumn("dbo.Developers", "lname");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Developers", "lname", c => c.String());
            AddColumn("dbo.Developers", "fname", c => c.String());
            DropColumn("dbo.Developers", "DeveloperLastName");
            DropColumn("dbo.Developers", "DeveloperFirstName");
        }
    }
}
