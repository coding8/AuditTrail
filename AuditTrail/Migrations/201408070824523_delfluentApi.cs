namespace AuditTrail.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class delfluentApi : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Books", "Published", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Books", "Published", c => c.DateTime(nullable: false, storeType: "date"));
        }
    }
}
