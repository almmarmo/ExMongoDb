using System;

namespace TesteMongo
{
    public class AuditRequest
    {
        public AuditRequest()
        {
            Date = DateTime.Now;
        }
        public int AuditId { get; set; }
        public DateTime Date { get; set; }
    }
}
