using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO.Movie;
using Shared.Common;

namespace Application.Interface.Services
{
     public interface IWatchListService
    {
        public Task<WatchListDto> AddAsync( Guid movieId);
        public Task<ResponseData> RemoveAsync( Guid movieId);
        public Task<List<MovieWatchListDto>> GetAllAsync();
    }
}
