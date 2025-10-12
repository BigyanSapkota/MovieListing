using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
     public class DeviceToken
    {
        public Guid Id{ get; set; } 
        public string? UserId { get; set; }
        public string Token { get; set; } 
        public string Platform { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.Now;

    }

    public class RegisterDeviceTokenDto
    {
        public string? UserId { get; set; }
        public string Token { get; set; }
        public string Platform { get; set; }
    }

}
