using System;

namespace AuditTrail.Models
{
    public class Audit
    {
        public long Id { get; set; }
        public string TableName { get; set; }
        public string UserId { get; set; }
        public string Actions { get; set; }
        public string OldData { get; set; }
        public string NewData { get; set; }
        public string TableIdValue { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    }
}