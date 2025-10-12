using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Entities
{
     public class BillType
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;

        public int interval { get; set; }
        public  bool IsFixedAmount { get; set; }
        public decimal DefaultAmount { get; set; }
        // Navigation property
        [JsonIgnore]
        public ICollection<Bill> Bills { get; set; } = new List<Bill>();
    }
}
