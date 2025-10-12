using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
     public class Rating : BaseEntity<Guid>
    {
        //public int RatingId { get; set; }   
        public Guid MovieId { get; set; }
        //public string UserId { get; set; }
        public int RatingValue { get; set; }
        //public DateTime CreatedDate { get; set; }



        public Movie Movie { get; set; }
        public User User { get; set; }


    }
}
