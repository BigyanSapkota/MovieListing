using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Movie
{
     public class CreateGenreDto
    {
        public string GenreName { get; set; }
    }

    public class UpdateGenreDto
    {
        public Guid GenreId { get; set; }
        public string GenreName { get; set; }
    }


    public class  GenreDto
    {
        public Guid GenreId { get;set; }
        public string GenreName { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? DeletedBy { get; set; }
        public string? DeletedAt { get; set; }

    }
}
