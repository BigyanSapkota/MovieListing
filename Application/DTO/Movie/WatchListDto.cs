using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Movie
{
     public class WatchListDto
    {
        public Guid MovieId { get; set; }
        public DateTime AddedAt { get; set; }

    }
    public class AddWatchListDto
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public Guid MovieId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class MovieWatchListDto
    {
       
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime ReleaseDate { get; set; }
        public List<string> Genres { get; set; }
        public List<string> Languages { get; set; }
        public string PosterUrl { get; set; }
        public double Rating { get; set; }
        public string Duration { get; set; } 
        public string YouTubeLink { get; set; }
        public DateTime AddedAt { get; set; }
    }
}
