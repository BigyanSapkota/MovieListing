using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PaymentTransaction
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; } = null!;
        public User User { get; set; } = null!;

        public List<Guid> BillIds { get; set; } = new List<Guid>();
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }  // Pending / Completed / Failed
        public string Pidx { get; set; } = null!; // Khalti payment token / reference
        public string? TransactionId { get; set; } // Khalti transaction ID after verification
        public string? BillType { get; set; } 
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public DateTime? CompletedAt { get; set; }
        public  string Transaction_Method { get; set; }
        public Guid? OrganizationId { get; set; }
        public Organization Organization { get; set; }
    }

   


}
