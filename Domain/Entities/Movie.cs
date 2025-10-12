using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
     public class Movie : BaseEntity<Guid>
    {
        //public int Id { get; set; }
        public string Title { get; set; }
        //public string Genre { get; set; }
        //public string Language { get; set; }
        public string Duration { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Description { get; set; }
        public double AverageRating { get; set; }
        public string PosterUrl { get; set; }
        public string YouTubeLink { get; set; }
        //public bool IsActive { get; set; }



        //public string? UserId { get; set; }
        public User User { get; set; }


        public ICollection<Comment> Comment { get; set; }
        public ICollection<Rating> Rating { get; set; }
        public ICollection<MovieGenre> MovieGenre { get; set; }
        public ICollection<MovieLanguage> MovieLanguage { get; set; }
        public ICollection<WatchList> WatchList { get; set; } = new List<WatchList>();



    }
}
