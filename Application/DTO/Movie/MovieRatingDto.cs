using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Movie
{
     public class MovieRatingDto
    {
        public Guid MovieId { get; set; }
        public int Rating { get; set; }

    }
}
