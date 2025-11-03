using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Application.DTO.Movie;
using Application.Interface.Repository;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Common;
using Shared.Helper;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Infrastructure.Repositories
{

     public class MovieRepo : IMovieRepo
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public MovieRepo(ApplicationDbContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public Task<List<Genre>> GetByGenreNameAsync(List<string> name)
        {

            return _context.Genre.Where(g => name.Contains(g.GenreName)).ToListAsync();
        }



        public Task<List<Language>> GetByLanguageNameAsync(List<string> name)
        {
            return _context.Language.Where(l => name.Contains(l.LanguageName)).ToListAsync();
        }


        public async Task<ResponseData<MovieDto>> GetById(Guid movieId)
        {
          var data =await _context.Movies.Include(m => m.MovieGenre).ThenInclude(mg => mg.Genre)
                                    .Include(m => m.MovieLanguage).ThenInclude(ml => ml.Language)
                                    .FirstOrDefaultAsync(m => m.Id == movieId && m.IsActive);

            if (data == null)
            {
                return new ResponseData<MovieDto>
                {
                    Success = false,
                    Message = "Movie not found",
                    Data = null
                };
            }

            
            
            var movieDto = _mapper.Map<MovieDto>(data);
            movieDto.PosterLink = data.PosterUrl;
            movieDto.Genre = data.MovieGenre?.Select(mg => mg.Genre.GenreName).ToList() ?? new List<string>();
            movieDto.Language = data.MovieLanguage?.Select(ml => ml.Language.LanguageName).ToList() ?? new List<string>();


            return new ResponseData<MovieDto>
            {
                Success = true,
                Message = "Movie retrieved successfully",
                Data = movieDto
            };
        }



        public async Task<ResponseData<List<MovieShowDto>>> GetMovieByFilterAsync(MovieFilterDto filterDto)
        {
            var query = _context.Movies
                .Where(m => m.IsActive);


            if (!string.IsNullOrEmpty(filterDto.MovieName))
            {
                var name = NormalizeHelper.NormalizeText(filterDto.MovieName);
                //var name = NormalizeHelper.NormalizeText(filterDto.MovieName);
                //var name = filterDto.MovieName.ToLower();
                query = query.Where(m => m.Title.ToLower().Replace(" ","").Replace(" ","-").Replace("+","").Replace("=","").Replace("_","").Contains(name));
            }

            if (filterDto.ReleaseDateFrom.HasValue)
                query = query.Where(m => m.ReleaseDate >= filterDto.ReleaseDateFrom.Value);

            if (filterDto.ReleaseDateTo.HasValue)
                query = query.Where(m => m.ReleaseDate <= filterDto.ReleaseDateTo.Value);


            if (filterDto.MinRating.HasValue)
                query = query.Where(m => m.Rating.Any())
                             .Where(m => m.Rating.Average(r => (double)r.RatingValue) >= filterDto.MinRating.Value);

            if (filterDto.MaxRating.HasValue)
                query = query.Where(m => m.Rating.Any())
                             .Where(m => m.Rating.Average(r => (double)r.RatingValue) <= filterDto.MaxRating.Value);


            if (!string.IsNullOrEmpty(filterDto.Genre))
            {
                var genre = NormalizeHelper.NormalizeText(filterDto.Genre);
                query = query.Where(m => m.MovieGenre.Any(g => g.Genre.GenreName.ToLower().Replace(" ", "").Replace(" ", "-").Replace("+", "").Replace("=", "").Replace("_", "") == genre));
            }


            if (!string.IsNullOrEmpty(filterDto.Language))
            {
                var language = NormalizeHelper.NormalizeText(filterDto.Language);
                query = query.Where(m => m.MovieLanguage.Any(l => l.Language.LanguageName.ToLower().Replace(" ", "").Replace(" ", "-").Replace("+", "").Replace("=", "").Replace("_", "") == language));
            }


            var data = await query
                .Select(m => new MovieShowDto
                {
                    Id = m.Id,
                    Title = m.Title,
                    Duration = m.Duration,
                    ReleaseDate = m.ReleaseDate,
                    Description = m.Description,
                    PosterUrl = m.PosterUrl,
                    YouTubeLink = m.YouTubeLink,

                    Genre = m.MovieGenre
                        .Select(g => g.Genre.GenreName)
                        .ToList(),

                    Language = m.MovieLanguage
                        .Select(l => l.Language.LanguageName)
                        .ToList(),

                    AverageRating = m.Rating.Any()
                       ? m.Rating.Average(r => (double)r.RatingValue)
                       : (double?)null,

                    TotalRatings = m.Rating.Count()


                })
                .ToListAsync();

            return new ResponseData<List<MovieShowDto>>
            {
                Success = data.Any(),
                Message = data.Any() ? "Movies retrieved successfully" : "No movies found",
                Data = data
            };
        }


        public async Task<ResponseData<PageResult<MovieShowDto>>> GetAllMovies(int pageNumber, int pageSize)
        {
            var data = _context.Movies.Where(m => m.IsActive).Include(m=>m.MovieGenre).ThenInclude(mg=>mg.Genre)
                                                             .Include(m => m.MovieLanguage).ThenInclude(ml => ml.Language)
                                                             .Include(m => m.Rating);
            var totalCount = await data.CountAsync();
            var movies = await data.OrderBy(m => m.ReleaseDate)
                                   .Skip((pageNumber - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();




            var movieDto = movies.Select(m => new MovieShowDto
            {
                Id = m.Id,
                Title = m.Title,
                Duration = m.Duration,
                ReleaseDate = m.ReleaseDate,
                Description = m.Description,
                PosterUrl = m.PosterUrl,
                YouTubeLink = m.YouTubeLink,
                Genre = m.MovieGenre.Select(g => g.Genre.GenreName).ToList(),
                Language = m.MovieLanguage.Select(l => l.Language.LanguageName).ToList(),
                AverageRating = m.AverageRating,
                TotalRatings = m.Rating.Count()

            }).ToList();

            return new ResponseData<PageResult<MovieShowDto>>
            {
                Success = true,
                Message = movieDto.Any() ? "Movies retrieved successfully" : "No movies found",
                Data = new PageResult<MovieShowDto>
                {
                    Items = movieDto,
                    TotalItems = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                }
            };
        }

        public async Task DeleteAsync(Guid movieId,string requestedByAdminId)
        {
            //movie.IsActive = false;
            //await _context.SaveChangesAsync();
            await _context.Movies.Where(m => m.Id == movieId)
                                 .ExecuteUpdateAsync(m => m.SetProperty(p => p.IsActive, false)
                                 .SetProperty(p => p.DeletedBy, requestedByAdminId)
                                 .SetProperty(p => p.DeletedAt, DateTime.Now));
        }


        public async Task<ResponseData<Movie>> GetMovieByIdAsync(Guid movieId)
        {
            var data = await _context.Movies.Include(m => m.MovieGenre).ThenInclude(mg => mg.Genre)
                                    .Include(m => m.MovieLanguage).ThenInclude(ml => ml.Language)
                                    .FirstOrDefaultAsync(m => m.Id == movieId && m.IsActive);

            if (data == null)
            {
                return new ResponseData<Movie>
                {
                    Success = false,
                    Message = "Movie not found",
                    Data = null
                };
            }



            return new ResponseData<Movie>
            {
                Success = true,
                Message = "Movie retrieved successfully",
                Data = data
            };
        }


        public async Task Update(Movie movie)
        {
            _context.Movies.Update(movie);
            await _context.SaveChangesAsync();
        }



    }
}
