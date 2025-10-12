using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
     public class BillDTO
    {
        public Guid Id { get; set; } = new Guid();
        public Guid BillTypeId { get; set; }
        public string UserId { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? PaidAt { get; set; }
        //public int BillingMonth { get; set; }
        //public int BillingYear { get; set; }
        //public DateTime? NextGenerateDate { get; set; }
    }


    public class CreateBill
    {
        public Guid BillTypeId { get; set; }
        public string UserId { get; set; } 
        public decimal TotalAmount { get; set; }
        public int BillingMonth { get; set; }
        public int BillingYear { get; set; }
        public DateTime? CreatedAt { get; set; }
    }


}
