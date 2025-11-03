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
using Shared.Common;
using Shared.Helper;

namespace Application.Service
{
     public class GenreService : IGenreService
    {
        private readonly IGenericRepo<Genre, Guid> _genreRepo;
        private readonly IGenreRepo _genreCustomRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;
        public GenreService(IGenericRepo<Genre, Guid> genreRepo,IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor, IGenreRepo genreCustomRepo)
        {
            _genreRepo = genreRepo;
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
            _genreCustomRepo = genreCustomRepo;
        }


        public async Task<ResponseData<GenreDto>> AddGenre(CreateGenreDto dto)
        {
            var user = _contextAccessor.HttpContext?.User;
            string? loggedInUser = UserInfoHelper.GetUserId(user);
            if (loggedInUser == null)
            {
                return new ResponseData<GenreDto> { Success = false, Message = "User not authenticated" };
            }

            var allGenre = await _genreRepo.GetAllAsync();
            if(allGenre.Any(g => g.GenreName.ToLower() == dto.GenreName.ToLower()))
            {
                return new ResponseData<GenreDto>
                {
                    Success = false,
                    Message = "Genre Already Exists"
                };
            }

            var genre = new Genre
            {
                Id = Guid.NewGuid(),
                GenreName = dto.GenreName,
                CreatedBy = loggedInUser,
                CreatedAt = DateTime.Now
            };

            var result = await _genreRepo.AddAsync(genre);
            if ((result.Success) || (result.Data != null))
            {
                await _unitOfWork.CommitAsync();

                var genreDto = new GenreDto
                {
                    GenreId = result.Data.Id,
                    GenreName = result.Data.GenreName,
                    CreatedBy = result.Data.CreatedBy,
                    CreatedAt = result.Data.CreatedAt
                };
                return new ResponseData<GenreDto>
                {
                    Success = true,
                    Message = "Genre added successfully",
                    Data = genreDto
                };
            }
            return new ResponseData<GenreDto>
            {
                Success = false,
                Message = "Failed to add genre",
                Data = null
            };


        }





        public async Task<ResponseData<bool>> DeleteGenre(Guid id)
        {
            var user = _contextAccessor.HttpContext?.User;
            var loggedInUser = UserInfoHelper.GetUserId(user);
            var response =await _genreRepo.GetByIdAsync(id);
            if(response.Data != null)
            {
                response.Data.DeletedBy = loggedInUser;
                response.Data.DeletedAt = DateTime.Now;
                var deleteresponse = await _genreRepo.DeleteAsync(response.Data);
                if(deleteresponse.Success)
                {
                    await _unitOfWork.CommitAsync();
                    return new ResponseData<bool>
                    {
                        Success = true,
                        Message = "Genre deleted successfully",
                        
                    };
                }
            }
            return new ResponseData<bool>
            {
                Success = false,
                Message = "Genre Cannot be deleted",
                Data = false
            };
        }



        public async Task<ResponseData<PageResult<GenreDto>>> GetAllGenres(int pageNumber , int pageSize)
        {
            var response = await _genreCustomRepo.GetAllGenre(pageNumber,pageSize);
            if (response!=null)
            {
                var genre = new PageResult<GenreDto>
                {
                    Items = response.Data.Items.Select(g => new GenreDto
                    {
                        GenreId = g.Id,
                        GenreName = g.GenreName,
                        CreatedBy = g.CreatedBy,
                        CreatedAt = g.CreatedAt
                    }).ToList(),
                    TotalItems = response.Data.TotalItems,
                    PageNumber = response.Data.PageNumber,
                    PageSize = response.Data.PageSize
                };


                return new ResponseData<PageResult<GenreDto>>
                {
                    Success = true,
                    Message = "Genres retrieved successfully",
                    Data = genre
                };

            }
            return new ResponseData<PageResult<GenreDto>>
            {
                Success = false,
                Message = "No genres found",
                Data = null
            };
        }



        public async Task<ResponseData<GenreDto>> GetGenreById(Guid id)
        {
            var response = await _genreRepo.GetByIdAsync(id);
            if (response.Success) 
            {
                var genre = new GenreDto
                {
                    GenreId = response.Data.Id,
                    GenreName = response.Data.GenreName,
                    CreatedBy = response.Data.CreatedBy,
                    CreatedAt = response.Data.CreatedAt
                };

                return new ResponseData<GenreDto>
                {
                    Success = true,
                    Message = "Genre retrieved successfully",
                    Data = genre
                };

            }
            return new ResponseData<GenreDto>
            {
                Success = false,
                Message = "Genre not found",
                Data = null
            };
        }



        public async Task<ResponseData<GenreDto>> UpdateGenre(UpdateGenreDto dto)
        {
            var result = await _genreRepo.GetByIdAsync(dto.GenreId);
            if(result.Success && result.Data != null)
            {
                var genre = result.Data;
                genre.GenreName = dto.GenreName;
                genre.UpdatedBy = UserInfoHelper.GetUserId(_contextAccessor.HttpContext?.User);
                genre.UpdatedAt = DateTime.Now;
                var updateResponse = await _genreRepo.UpdateAsync(genre);
                if (updateResponse.Success)
                {
                    await _unitOfWork.CommitAsync();
                    var data = new GenreDto
                    {
                        GenreId = genre.Id,
                        GenreName = genre.GenreName,
                        CreatedBy = genre.CreatedBy,
                        CreatedAt = genre.CreatedAt
                    };

                    return new ResponseData<GenreDto>
                    {
                        Success = true,
                        Message = "Genre updated successfully",
                        Data = data
                        
                    };
                }
            }
            return new ResponseData<GenreDto>
            {
                Success = false,
                Message = "Failed to update genre",
                Data = null
            };
        }


        public async Task<ResponseData<int>> GetActiveGenreCount()
        {
            var count = await _genreRepo.GetCount(g => g.IsActive);
            return new ResponseData<int>
            {
                Success = true,
                Message = "Active genre count retrieved successfully",
                Data = count
            };
        }




    }
}
