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
     public interface IWatchListRepo
    {
        Task<bool> ExistsAsync(string userId, Guid movieId);
        Task<ResponseData> AddAsync(WatchList watchlist);
        Task RemoveAsync(WatchList watchlist);
        Task<WatchList?> GetAsync(string userId, Guid movieId);
        Task<List<MovieWatchListDto>> GetAllAsync(string userId);
    }
}
