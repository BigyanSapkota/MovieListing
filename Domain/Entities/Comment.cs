using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
     public class Comment : BaseEntity<Guid>
    {

        //public int CommentId { get; set; }
        public Guid MovieId { get; set; }
        //public string UserId { get; set; }
        public string Content { get; set; }
        //public DateTime CreatedDate { get; set; }



        public Movie Movie { get; set; }
        public User User { get; set; }




    }
}
