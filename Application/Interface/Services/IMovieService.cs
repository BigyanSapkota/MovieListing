using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO.Movie;
using Domain.Entities;
using Shared.Common;

namespace Application.Interface.Services
{
     public interface IMovieService
    {
        public Task<ResponseData<MovieDto>> AddMovieAsync(CreateMovieDto movieDto,string loginuser);
        public Task<ResponseData<PageResult<MovieShowDto>>> GetAllMoviesAsync(int pageNumber, int pageSize);
        public Task<ResponseData<MovieDto>> GetMovieByIdAsync(Guid id);
        public Task<ResponseData<MovieDto>> UpdateMovieAsync(MovieDto movieDto,string loginuser);
        public Task<ResponseData<MovieDto>> DeleteMovieAsync(Guid id,string loginuser);
        public Task<ResponseData<List<MovieShowDto>>> GetMovieByFilterAsync(MovieFilterDto filterDto);
        public Task<ResponseData> ShareMovieAsync(Guid movieId, string email,string loginUser,string userEmail);
        public Task<ResponseData<int>> GetAllMovieCount();
        public Task<ResponseData<int>> GetActiveMovieCount();
    


    }
}
