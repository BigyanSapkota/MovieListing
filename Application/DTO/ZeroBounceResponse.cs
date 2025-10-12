using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
     public class ZeroBounceResponse
    {
        public string Address { get; set; }
        public string Status { get; set; }
        public string SubStatus { get; set; }
        public bool FreeEmail { get; set; }
        public string Domain { get; set; }
        public bool MXFound { get; set; }

        // Optional additional fields
        public string Account { get; set; }
        public string DidYouMean { get; set; }
        public string DomainAgeDays { get; set; }
        public string EmailType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
    }

    public class ZeroBounceRequest
    {
        public string email { get; set; }
    }

    public class EmailRequest
    {
        public string Email { get; set; }
    }

}
