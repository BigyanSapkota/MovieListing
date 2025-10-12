using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Movie
{
     public class MovieCommentDto
    {
        public Guid MovieId { get; set; }
        public string Content { get; set; }

    }

    public class CommentDto
    {
        public Guid Id { get; set; }
        public Guid MovieId { get; set; }
        public string Content { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
    }


    public class UpdateCommentDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
    }



}
