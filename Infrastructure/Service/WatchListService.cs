using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO.Movie;
using Application.Interface;
using Application.Interface.Repository;
using Application.Interface.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.Common;
using Shared.Helper;

namespace Application.Service
{
     public class WatchListService : IWatchListService
    {
       
        private readonly  IWatchListRepo _repo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;
        public WatchListService(IWatchListRepo repo,IUnitOfWork unitOfWork,IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _repo = repo;
            _contextAccessor = contextAccessor;
        }


        public async Task<WatchListDto> AddAsync(Guid movieId)
        {
            var user = _contextAccessor.HttpContext?.User;
            string? userId = UserInfoHelper.GetUserId(user);
            if (userId == null)
            {
                throw new Exception("User not authenticated");
            }

            var data = await _repo.ExistsAsync(userId, movieId);
            if (!data)
            {
                var watchlist = new WatchList
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    MovieId = movieId,
                    AddedAt = DateTime.Now
                };
                var result = await _repo.AddAsync(watchlist);
                if (result.Success)
                {
                    result.Data = watchlist;
                    var watchlistDto = new WatchListDto
                    {
                        MovieId = watchlist.MovieId,
                        AddedAt = watchlist.AddedAt

                    };
                    return (watchlistDto);
                }
            }
            return null ;
        }



        public async Task<List<MovieWatchListDto>> GetAllAsync()
        {
            var user = _contextAccessor.HttpContext?.User;
            string? userId = UserInfoHelper.GetUserId(user);
            if (userId == null)
            {
                throw new Exception("User not authenticated");
            }

            var watchlist = await _repo.GetAllAsync(userId);
            return watchlist ?? new List<MovieWatchListDto>();
        }




        public async Task<ResponseData> RemoveAsync(Guid movieId)
        {
            var user = _contextAccessor.HttpContext?.User;
            string? userId = UserInfoHelper.GetUserId(user);
            if (userId == null)
            {
                throw new Exception("User not authenticated");
            }

            var watchlist = await _repo.GetAsync(userId, movieId);
            if (watchlist != null)
            {
                await _repo.RemoveAsync(watchlist);
                return new ResponseData
                {
                    Success = true,
                    Message = "Removed from WatchList"
                };
            }

            return new ResponseData
            {
                Success = false,
                Message = "WatchList item not found"
            };

        }



    }
}
