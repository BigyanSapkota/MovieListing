using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Movie
{
     public class RatingDto
    {
        public Guid Id { get; set; }
        public Guid MovieId { get; set; }
        public int Rating { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
