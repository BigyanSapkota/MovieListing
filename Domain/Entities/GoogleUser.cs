using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
     public class GoogleUser : BaseEntity<Guid>
    {
        public string Email { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Picture { get; set; } = default!;
    }
}
