using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO.Movie;
using Domain.Entities;
using Shared.Common;

namespace Application.Interface.Repository
{
     public interface IMovieRatingRepo
    {
        public Task<double> GetAverageRatingAsync(Guid movieId);
        public Task AddOrUpdateRatingAsync(MovieRatingDto dto, string userId);
        public Task UpdateAverageRatingAsync(Guid movieId, double avgRating);
        public Task<ResponseData<PageResult<Rating>>> GetMovieAllRating(Guid movieId,int pageNumber, int pageSize);
    }
}
