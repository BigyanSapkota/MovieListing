using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Payment
{
    public class PaymentRequestForEsewa
    {
            public decimal Amount { get; set; }
            public decimal TaxAmount { get; set; } = 0;
          public decimal ServiceCharge { get; set; } = 1;
            public decimal DeliveryCharge { get; set; } = 0;
           public string TransactionUUID { get; set; }
            public string ProductCode { get; set; }
          public string SuccessUrl { get; set; } = "https://localhost:5001/api/payment/success";
          public string FailureUrl { get; set; } = "https://localhost:5001/api/payment/failure";
    }


    public class PaymentRequests
    {
        public decimal Amount { get; set; }
        //public string TransactionId { get; set; } = Guid.NewGuid().ToString();
    }


    public class PaymentResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string? PaymentUrl { get; set; }
        public Dictionary<string, string>? Payload { get; set; }
        
    }


    public class PaymentResult
    {
        public bool Success { get; set; }
        public string ReferenceId { get; set; } = null!;
        public decimal Amount { get; set; }
        public string Transaction_Uuid { get; set; } = null!;
        public string ProductCode { get; set; } = null!;
        public string Message { get; set; } = null!;
        public List<BillDetails> Bill_Details { get; set; }
    }


}
