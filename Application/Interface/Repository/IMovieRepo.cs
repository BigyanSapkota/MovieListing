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
     public interface IMovieRepo
    {
        public Task<List<Genre>> GetByGenreNameAsync(List<string> name);
        public Task<List<Language>> GetByLanguageNameAsync(List<string> name);
        public Task<ResponseData<List<MovieShowDto>>> GetMovieByFilterAsync(MovieFilterDto filterDto);
        public Task<ResponseData<MovieDto>> GetById(Guid movieId);
        public Task<ResponseData<PageResult<MovieShowDto>>> GetAllMovies(int pageNumber, int pageSize);


    }
}
