using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
     public class WatchList
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public Guid MovieId { get; set; }
        public DateTime AddedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsActive { get; set; } = true;


        public virtual User User { get; set; } 
        public virtual Movie Movie { get; set; }

    }
}
