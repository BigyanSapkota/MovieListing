using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
     public class PaymentHistory
    {
        public int Id { get; set; }
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public string Payload { get; set; } // raw JSON for audit
        public DateTime CreatedAt { get; set; } = DateTime.Now ;

    }
}
