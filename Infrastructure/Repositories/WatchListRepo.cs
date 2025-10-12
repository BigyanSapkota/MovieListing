using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO.Movie;
using Application.Interface.Repository;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Common;

namespace Infrastructure.Repositories
{
     public class WatchListRepo : IWatchListRepo
    {
        private readonly ApplicationDbContext _context;
        public WatchListRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(string userId, Guid movieId)
        {
            return await _context.WatchList.Where(w => w.UserId == userId && w.MovieId == movieId).AnyAsync();
            
        }

        public async Task<ResponseData> AddAsync(WatchList watchlist)
        {
            await _context.WatchList.AddAsync(watchlist);
            await _context.SaveChangesAsync();
            return new ResponseData
            {
                Success = true,
                Message = "Added to WatchList",
                Data = watchlist
            };
        }

        public async Task RemoveAsync(WatchList watchlist)
        {
            
            watchlist.IsActive = false;
            await _context.SaveChangesAsync();
        }

        public async Task<WatchList?> GetAsync(string userId, Guid movieId)
        {
            return await _context.WatchList
                .Where(w => w.UserId == userId && w.MovieId == movieId)
                .FirstOrDefaultAsync();
        }


        public Task<List<MovieWatchListDto>> GetAllAsync(string userId)
        {
            var watchlist = _context.WatchList
                .Where(w => w.UserId == userId && w.IsActive)
                .Include(w => w.Movie)
                .ThenInclude(m => m.MovieGenre)
                .ThenInclude(m => m.Genre)
                .Include(w => w.Movie)
                .ThenInclude(m => m.MovieLanguage)
                .ThenInclude(m => m.Language)
                .Select(w => new MovieWatchListDto
                {
                    Title = w.Movie.Title,
                    Description = w.Movie.Description,
                    ReleaseDate = w.Movie.ReleaseDate,
                    Genres = w.Movie.MovieGenre.Select(g => g.Genre.GenreName).ToList(),
                    Languages = w.Movie.MovieLanguage.Select(l => l.Language.LanguageName).ToList(),
                    PosterUrl = w.Movie.PosterUrl,
                    Rating = w.Movie.AverageRating,
                    Duration = w.Movie.Duration,
                    YouTubeLink = w.Movie.YouTubeLink,
                    AddedAt = w.AddedAt,
                })
                .ToListAsync();

            return watchlist;
        }




    }
}
