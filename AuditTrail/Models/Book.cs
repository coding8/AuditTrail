using System;
using System.ComponentModel.DataAnnotations;

namespace AuditTrail.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }

        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayFormat(DataFormatString = "{0:d}",ApplyFormatInEditMode = true)]
        public DateTime Published { get; set; }
    }
}