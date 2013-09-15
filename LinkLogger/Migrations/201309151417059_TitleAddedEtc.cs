namespace LinkLogger.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TitleAddedEtc : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Links",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Url = c.String(),
                        Title = c.String(),
                        User = c.String(),
                        Channel = c.String(),
                        RegisteredAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Links");
        }
    }
}
