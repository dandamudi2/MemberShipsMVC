namespace MemberShipWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addSubscriptionTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 25),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Subscription",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 255),
                        Description = c.String(maxLength: 2048),
                        RegistrationCode = c.String(maxLength: 20),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Subscription");
            DropTable("dbo.ProductType");
        }
    }
}
