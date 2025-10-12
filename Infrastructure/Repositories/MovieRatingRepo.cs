using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO.Movie;
using Application.Interface;
using Application.Interface.Repository;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Common;

namespace Infrastructure.Repositories
{
     public class MovieRatingRepo : IMovieRatingRepo
    {
        private readonly IGenericRepo<Rating, Guid> _ratingRepo;
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        public MovieRatingRepo(ApplicationDbContext context,IUnitOfWork unitOfWork, IGenericRepo<Rating, Guid> ratingRepo)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _ratingRepo = ratingRepo;
        }

        public async Task<double> GetAverageRatingAsync(Guid movieId)
        {
            return await _context.Rating.Where(x => x.MovieId == movieId).AverageAsync(y => (double)y.RatingValue);
            
        }


        public async Task AddOrUpdateRatingAsync(MovieRatingDto dto, string userId)
        {
            var existingRating = await _context.Rating.FirstOrDefaultAsync(r => r.MovieId == dto.MovieId && r.CreatedBy == userId);

            if (existingRating != null)
            {
                existingRating.RatingValue = dto.Rating;
                existingRating.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                var rating = new Rating
                {
                    Id = Guid.NewGuid(),
                    MovieId = dto.MovieId,
                    CreatedBy = userId,
                    CreatedAt = DateTime.Now,
                    RatingValue = dto.Rating,           
                };
               
                await _ratingRepo.AddAsync(rating);
            }
            await _unitOfWork.CommitAsync();
        }




        public async Task UpdateAverageRatingAsync(Guid movieId, double avgRating)
        {
            var movie = await _context.Movies.Where(x => x.Id == movieId).FirstOrDefaultAsync();

            if (movie != null)
            {
                movie.AverageRating = avgRating;
                await _unitOfWork.CommitAsync();
            }
        }




        public async Task<ResponseData<PageResult<Rating>>> GetMovieAllRating(Guid movieId,int pageNumber, int pageSize)
        {
            var data = _context.Rating.Where(x => x.MovieId == movieId && x.IsActive );
            var totalCount = await data.CountAsync();
            var ratings = await data.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            
            var result = new PageResult<Rating>
            {
                Items = ratings,
                TotalItems = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return new ResponseData<PageResult<Rating>>
            {
                Success = true,
                Message = ratings.Any() ? "Ratings retrieved successfully" : "No ratings found for this movie",
                Data = result
            };


        }
    }
}
