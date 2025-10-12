using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
     public class Organization
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string ContactEmail { get; set; } = null!;
        public string ContactPhone { get; set; } = null!;
        public bool IsActive { get; set; } = true;

        public string SecretKey { get; set; } = null!;

        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<PaymentTransaction> PaymentTransaction { get; set; } = new List<PaymentTransaction>();
    }
}
