using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
     public class ElectricityBill
    {
        [Key]
        public Guid Id { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }


        public string type { get; set; } = "Electricity Bill Payment";

        public int Year { get; set; }     
        public int Month { get; set; }    

        public decimal Amount { get; set; }
        public bool IsPaid { get; set; } = false;
        public string? TransactionId { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public DateTime? PaidAt { get; set; }

    }
}
