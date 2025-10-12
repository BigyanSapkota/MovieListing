using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
     public class Bill : BaseEntity<Guid>
    {
        //public Guid Id { get; set; }
        public Guid BillTypeId { get; set; }
        public string UserId { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? PaidAt { get; set; }

       



        // Navigation properties
        public BillType BillType { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
