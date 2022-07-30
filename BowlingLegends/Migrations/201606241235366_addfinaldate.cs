namespace BowlingLegends.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addfinaldate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Season", "FinalDate", c => c.DateTime(nullable: false, storeType: "date"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Season", "FinalDate");
        }
    }
}
