namespace LanAdeptData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewVersion : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Places", "PlaceSectionID", "dbo.PlaceSections");
            DropIndex("dbo.Places", new[] { "PlaceSectionID" });
            AddColumn("dbo.Places", "SeatsId", c => c.String());
            DropColumn("dbo.Places", "Number");
            DropColumn("dbo.Places", "PlaceSectionID");

        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.PlaceSections",
                c => new
                    {
                        PlaceSectionID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.PlaceSectionID);
            
            AddColumn("dbo.Places", "PlaceSectionID", c => c.Int(nullable: false));
            AddColumn("dbo.Places", "Number", c => c.Int(nullable: false));
            DropColumn("dbo.Places", "SeatsId");
            CreateIndex("dbo.Places", "PlaceSectionID");
            AddForeignKey("dbo.Places", "PlaceSectionID", "dbo.PlaceSections", "PlaceSectionID");
        }
    }
}
