using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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
        public string BillTypeName { get; set; }
        public string UserId { get; set; } 
        public decimal TotalAmount { get; set; }
        public int BillingMonth { get; set; }
        public int BillingYear { get; set; }
        public DateTime? CreatedAt { get; set; }
    }




    public class BillPdfDto
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public Guid BillTypeId { get; set; }
        public string BillTypeName { get; set; }
        public int BillingMonth { get; set; }
        public int BillingYear { get; set; }
        [Precision(18,4)]
        public decimal TotalAmount { get; set; }
        public DateTime? CreatedAt { get;set; }


    }


    public class BillSummaryDto
    {
        public List<CreateBill> PreviousBills { get; set; } = new();
        public List<CreateBill> NewBills { get; set; } = new();
    }



}
