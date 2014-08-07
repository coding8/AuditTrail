using AuditTrail.Models;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Web;

namespace AuditTrail.Helper
{
    public class AuditTrailFactory
    {
        //字段
        private DbContext context;

        //构造函数
        public AuditTrailFactory(DbContext context)
        {
            this.context = context;
        }

        #region 方法

        //获得更改数据
        public Audit GetAudit(DbEntityEntry entry)
        {
            Audit audit = new Audit();
            //var user = (User)HttpContext.Current.Session[":user"];
            //audit.UserId = user.UserName;

            audit.UserId = HttpContext.Current.User.Identity.Name;
            audit.TableName = GetTableName(entry);
            audit.UpdateDate = DateTime.Now;
            audit.TableIdValue = GetKeyValue(entry);

            //entry is Added 
            if (entry.State == EntityState.Added)
            {
                var newValues = new StringBuilder();
                SetAddedProperties(entry, newValues);
                audit.NewData = newValues.ToString();
                audit.Actions = AuditActions.I.ToString();
            }
            //entry in deleted
            else if (entry.State == EntityState.Deleted)
            {
                var oldValues = new StringBuilder();
                SetDeletedProperties(entry, oldValues);
                audit.OldData = oldValues.ToString();
                audit.Actions = AuditActions.D.ToString();
            }
            //entry is modified
            else if (entry.State == EntityState.Modified)
            {
                var oldValues = new StringBuilder();
                var newValues = new StringBuilder();
                SetModifiedProperties(entry, oldValues, newValues);
                audit.OldData = oldValues.ToString();
                audit.NewData = newValues.ToString();
                audit.Actions = AuditActions.U.ToString();
            }

            return audit;
        }

        //获得数据表名称
        private string GetTableName(DbEntityEntry dbEntry)
        {
            TableAttribute tableAttr = dbEntry.Entity.GetType().GetCustomAttributes(typeof(TableAttribute), false).SingleOrDefault() as TableAttribute;
            string tableName = tableAttr != null ? tableAttr.Name : dbEntry.Entity.GetType().Name;
            return tableName;
        }

        //获得表的主键
        public string GetKeyValue(DbEntityEntry entry)
        {
            var objectStateEntry = ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.GetObjectStateEntry(entry.Entity);
            string pk = "0";
            if (objectStateEntry.EntityKey.EntityKeyValues != null)
                pk = objectStateEntry.EntityKey.EntityKeyValues[0].Value.ToString();

            return pk;
        }

        //添加
        private void SetAddedProperties(DbEntityEntry entry, StringBuilder newData)
        {
            foreach (var propertyName in entry.CurrentValues.PropertyNames)
            {
                var newVal = entry.CurrentValues[propertyName];
                if (newVal != null)
                {
                    newData.AppendFormat("{0}={1} || ", propertyName, newVal);
                }
            }
            if (newData.Length > 0)
                newData = newData.Remove(newData.Length - 3, 3);
        }

        //删除
        private void SetDeletedProperties(DbEntityEntry entry, StringBuilder oldData)
        {
            DbPropertyValues dbValues = entry.GetDatabaseValues();
            foreach (var propertyName in dbValues.PropertyNames)
            {
                var oldVal = dbValues[propertyName];
                if (oldVal != null)
                {
                    oldData.AppendFormat("{0}={1} || ", propertyName, oldVal);
                }
            }
            if (oldData.Length > 0)
                oldData = oldData.Remove(oldData.Length - 3, 3);
        }

        //更改
        private void SetModifiedProperties(DbEntityEntry entry, StringBuilder oldData, StringBuilder newData)
        {
            DbPropertyValues dbValues = entry.GetDatabaseValues();
            foreach (var propertyName in entry.OriginalValues.PropertyNames)
            {
                var oldVal = dbValues[propertyName];
                var newVal = entry.CurrentValues[propertyName];
                //if (oldVal != null && newVal != null && !Equals(oldVal, newVal))//若原来就是null值
                if (!Equals(oldVal, newVal))
                {
                    newData.AppendFormat("{0}={1} || ", propertyName, newVal);
                    oldData.AppendFormat("{0}={1} || ", propertyName, oldVal);
                }
            }
            if (oldData.Length > 0)
                oldData = oldData.Remove(oldData.Length - 3, 3);
            if (newData.Length > 0)
                newData = newData.Remove(newData.Length - 3, 3);
        }

        #endregion 方法
    }
    public enum AuditActions
    {
        I,
        U,
        D
    }
}