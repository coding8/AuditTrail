namespace AuditTrail.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class audit : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Audits",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TableName = c.String(),
                        UserId = c.String(),
                        Actions = c.String(),
                        OldData = c.String(),
                        NewData = c.String(),
                        TableIdValue = c.String(),
                        UpdateDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Audits");
        }
    }
}
