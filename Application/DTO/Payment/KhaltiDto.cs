using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MimeKit.Encodings;
using static System.Net.WebRequestMethods;

namespace Application.DTO.Payment
{
     public class KhaltiDto
    {
        //public string RedirectUrl { get; set; } = "https://localhost:5001";    
       public int Amount { get; set; }
    }

    public class PaymentRequest
    {
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
    }



    public class KhaltiInitiateResponse
    {
        public string pidx { get; set; }
        public string payment_url { get; set; }
        public string expires_at { get; set; }
        public int expires_in { get; set; }
        //public Guid BillId { get; set; }
        //public List<Guid> BillIds { get; set; } = new List<Guid>();
    }




    public class KhaltiVerifyResponse
    {
        public string pidx { get; set; }
        public decimal total_amount { get; set; }
        public string status { get; set; }
        public string transaction_id { get; set; }
        public decimal fee { get; set; }
        //public int OrderId { get; set; }
        //public bool refunded { get; set; }

    }


    public class BillDetails
    {
        public Guid BillId { get; set; }
        public string BillType { get; set; }
        public decimal Amount { get; set; }
    }
 

    public class KhaltiPaymentResultDto
    {
        public KhaltiVerifyResponse KhaltiResponse { get; set; } = null!;
        public List<BillDetails> Bill_detail { get; set; }
    }


}
