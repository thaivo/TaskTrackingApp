namespace TaskTrackingApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeveloperSkill : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SkillDevelopers",
                c => new
                    {
                        Skill_SkillID = c.Int(nullable: false),
                        Developer_DevID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Skill_SkillID, t.Developer_DevID })
                .ForeignKey("dbo.Skills", t => t.Skill_SkillID, cascadeDelete: true)
                .ForeignKey("dbo.Developers", t => t.Developer_DevID, cascadeDelete: true)
                .Index(t => t.Skill_SkillID)
                .Index(t => t.Developer_DevID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SkillDevelopers", "Developer_DevID", "dbo.Developers");
            DropForeignKey("dbo.SkillDevelopers", "Skill_SkillID", "dbo.Skills");
            DropIndex("dbo.SkillDevelopers", new[] { "Developer_DevID" });
            DropIndex("dbo.SkillDevelopers", new[] { "Skill_SkillID" });
            DropTable("dbo.SkillDevelopers");
        }
    }
}
