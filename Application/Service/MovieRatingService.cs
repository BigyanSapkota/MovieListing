using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO.Movie;
using Application.Interface;
using Application.Interface.Repository;
using Application.Interface.Services;
using AutoMapper;
using Domain.Entities;
using Shared.Common;

namespace Application.Service
{
    public class MovieRatingService : IMovieRatingService
    {
        private readonly IGenericRepo<Rating, Guid> _ratingRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMovieRatingRepo _repo;


        public MovieRatingService(IGenericRepo<Rating, Guid> ratingRepo, IUnitOfWork unitOfWork, IMapper mapper, IMovieRatingRepo repo)
        {
            _ratingRepo = ratingRepo;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _repo = repo;
        }

        public async Task<ResponseData<RatingDto>> AddMovieRatingAsync(MovieRatingDto dto, string UserId)
        {

            if (dto == null || dto.MovieId == Guid.Empty || dto.Rating < 1 || dto.Rating > 5)
            {
                return new ResponseData<RatingDto>
                {
                    Success = false,
                    Message = "Invalid rating data.",
                    Data = null
                };
            }

            await _repo.AddOrUpdateRatingAsync(dto, UserId);

            var avgRating = await _repo.GetAverageRatingAsync(dto.MovieId);

            await _repo.UpdateAverageRatingAsync(dto.MovieId, avgRating);

            return new ResponseData<RatingDto>
            {
                Success = true,
                Message = "Rating added successfully.",
                Data = new RatingDto { MovieId = dto.MovieId, Rating = dto.Rating }
            };
        }



        public async Task<double> GetAverageRatingAsync(Guid MovieId)
        {
            return await _repo.GetAverageRatingAsync(MovieId);
        }

        public async Task<ResponseData<PageResult<RatingDto>>> GetRatingByMovie(Guid MovieId, int pageNumber, int pageSize)
        {
            var response = await _repo.GetMovieAllRating(MovieId, pageNumber, pageSize);
            if (response.Success)
            {
                var ratings = new PageResult<RatingDto>
                {
                    Items = response.Data.Items.Select(r => new RatingDto
                    {
                        Id = r.Id,
                        MovieId = r.MovieId,
                        Rating = r.RatingValue,
                        CreatedBy = r.CreatedBy,
                        CreatedAt = r.CreatedAt
                    }).ToList(),
                    TotalItems = response.Data.TotalItems,
                    PageNumber = response.Data.PageNumber,
                    PageSize = response.Data.PageSize
                };
                return new ResponseData<PageResult<RatingDto>>
                {
                    Success = true,
                    Message = "Ratings retrieved successfully.",
                    Data = ratings
                };
            }

            return new ResponseData<PageResult<RatingDto>>
            {
                Success = false,
                Message = "Failed to retrieve ratings.",
                Data = null
            };
        }

     public async Task<ResponseData<int>> GetActiveRatingCount()
        {
            var count = await _ratingRepo.GetCount();
            return new ResponseData<int>
            {
                Success = true,
                Message = "Active rating count retrieved successfully.",
                Data = count
            };
        }



    }
}
