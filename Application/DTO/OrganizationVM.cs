using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
     public class OrganizationVM
    {
        public string Name { get; set; } = null!;
        public string ContactEmail { get; set; } = null!;
        public string ContactPhone { get; set; } = null!;
        public bool IsActive { get; set; } = true;

        public string SecretKey { get; set; } = null!;
        public int shouldApprovedBy { get; set; }
    }
}
