using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Application.DTO.Movie
{
     public class MovieDto
    {
        public Guid Id { get; set; } 
        public string Title { get; set; }
        public string Duration { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Description { get; set; }
        public IFormFile? PosterUrl { get; set; }
        public string? PosterLink { get; set; } 
        public string YouTubeLink { get; set; }
        public bool IsActive { get; } = true;


        public List<string> Genre { get; set; } = new();
        public List<string> Language { get; set; } = new();

    }


    public class CreateMovieDto
    {
        public string Title { get; set; }
        public string Duration { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Description { get; set; }
        public IFormFile PosterUrl { get; set; }
        public string YouTubeLink { get; set; }
        public bool IsActive { get; set; } = true;


        public List<string> Genre { get; set; } = new();
        public List<string> Language { get; set; } = new();


    }

    public class MovieShowDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Duration { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Description { get; set; }
        public string PosterUrl { get; set; }
        public string YouTubeLink { get; set; }
        public bool IsActive { get; } = true;


        public List<string> Genre { get; set; } = new();
        public List<string> Language { get; set; } = new();
        public double? AverageRating { get; set; } 
        public int TotalRatings { get; set; }

    }

    public class MovieFilterDto
    {
        public string? MovieName { get; set; }
        public string? Genre { get; set; }
        public double? MinRating { get; set; }
        public double? MaxRating { get; set; }
        public DateTime? ReleaseDateFrom { get; set; }
        public DateTime? ReleaseDateTo { get; set; }
        public string? Language { get; set; }


    }


    public class AIMovie
    {
        public string Title { get; set; }
        public string Genre { get; set; }
        public int Rating { get; set; } 
        public bool IsTop { get; set; }
    }






}
