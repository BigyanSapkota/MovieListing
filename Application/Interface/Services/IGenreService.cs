using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO.Movie;
using Shared.Common;

namespace Application.Interface.Services
{
     public interface IGenreService
    {
        public Task<ResponseData<GenreDto>> AddGenre(CreateGenreDto dto);
        public Task<ResponseData<bool>> DeleteGenre(Guid id);
        public Task<ResponseData<GenreDto>> UpdateGenre(UpdateGenreDto dto);
        public Task<ResponseData<GenreDto>> GetGenreById(Guid id);
        public Task<ResponseData<PageResult<GenreDto>>> GetAllGenres(int pageNumber,int pageSize);
        public Task<ResponseData<int>> GetActiveGenreCount();

    }
}
