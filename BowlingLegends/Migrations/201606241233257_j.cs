namespace BowlingLegends.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class j : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Season", "FinalDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Season", "FinalDate", c => c.DateTime(nullable: false));
        }
    }
}
