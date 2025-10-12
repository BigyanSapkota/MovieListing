using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
     public class NotificationRequest
    {
        public string Token { get; set; }

        public string Title { get; set; }
        public string Body { get; set; }
        //public string UserId { get; set; }
    }



    public class NotificationRequestMultiple
    {
        public List<string> Tokens { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
    }




    public class PushSubscriptionModel
    {
        public string Endpoint { get; set; }
        public string P256dh { get; set; }
        public string Auth { get; set; }
    }


}
