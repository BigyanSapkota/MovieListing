using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO.Movie;
using Shared.Common;

namespace Application.Interface.Services
{
     public interface IMovieRatingService
    {
        public Task<ResponseData<RatingDto>> AddMovieRatingAsync(MovieRatingDto dto, string UserId);
        public Task<double> GetAverageRatingAsync(Guid MovieId);
        public Task<ResponseData<PageResult<RatingDto>>> GetRatingByMovie(Guid MovieId,int pageNumber, int pageSize);
        public Task<ResponseData<int>> GetActiveRatingCount();
    }
}
