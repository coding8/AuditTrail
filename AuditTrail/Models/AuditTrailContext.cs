using AuditTrail.Helper;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace AuditTrail.Models
{
    public class AuditTrailContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx

        public AuditTrailContext()
            : base("name=AuditTrailContext")
        {
        }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Book>().Property(t => t.Published).HasColumnType("date");
        //}
        public DbSet<Book> Books { get; set; }

        public DbSet<Audit> Audit { get; set; }
        private AuditTrailFactory auditFactory;
        private List<Audit> auditList = new List<Audit>();
        private List<DbEntityEntry> list = new List<DbEntityEntry>();

        //重写方法
        public override int SaveChanges()
        {
            auditList.Clear();
            list.Clear();
            auditFactory = new AuditTrailFactory(this);

            var entityList = ChangeTracker.Entries().Where(p => p.State == EntityState.Added || p.State == EntityState.Deleted || p.State == EntityState.Modified);
            foreach (var entity in entityList)
            {
                Audit audit = auditFactory.GetAudit(entity);
                bool isValid = true;
                if (entity.State == EntityState.Modified && string.IsNullOrWhiteSpace(audit.NewData) && string.IsNullOrWhiteSpace(audit.OldData))
                {
                    isValid = false;
                }
                if (isValid)
                {
                    auditList.Add(audit);
                    list.Add(entity);
                }
            }

            var retVal = base.SaveChanges();
            if (auditList.Count > 0)
            {
                int i = 0;
                foreach (var audit in auditList)
                {
                    if (audit.Actions == AuditActions.I.ToString())
                        audit.TableIdValue = auditFactory.GetKeyValue(list[i]);
                    this.Audit.Add(audit);
                    i++;
                }

                base.SaveChanges();
            }
            return retVal;
        }
    }
}