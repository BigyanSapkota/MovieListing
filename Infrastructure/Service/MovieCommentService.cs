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
using Shared.Common;
using static Google.Apis.Requests.BatchRequest;

namespace Application.Service
{
     public class MovieCommentService : IMovieCommentService
    {
        private readonly IGenericRepo<Comment, Guid> _commentRepo;
        private readonly IMovieCommentRepo _repo;
        private readonly IUnitOfWork _unitOfWork;
        public MovieCommentService(IGenericRepo<Comment, Guid> commentRepo,IUnitOfWork unitOfWork, IMovieCommentRepo repo   )
        {
            _commentRepo = commentRepo;
            _unitOfWork = unitOfWork;
            _repo = repo;
        }

        public async Task<ResponseData<CommentDto>> AddMovieCommentAsync(MovieCommentDto dto, string UserId)
        {
            var data = new Comment
            {
                Id = Guid.NewGuid(),
                MovieId = dto.MovieId,
                Content = dto.Content,
                CreatedBy = UserId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            var result = await _commentRepo.AddAsync(data);
            if (result.Success == true)
            {
                await _unitOfWork.CommitAsync();
                var comment = new CommentDto
                {
                    Id = result.Data.Id,
                    MovieId = result.Data.MovieId,
                    Content = result.Data.Content,
                    CreatedBy = result.Data.CreatedBy,
                    CreatedAt = result.Data.CreatedAt
                };
                return new ResponseData<CommentDto>
                {
                    Success = true,
                    Message = "Comment added successfully",
                    Data = comment
                };
            }
            return new ResponseData<CommentDto>
            {
                Success = false,
                Message = "Failed to add comment",
                Data = null
            };
        }



        public async Task<ResponseData<CommentDto>> DeleteMovieCommentAsync(Guid CommentId, string UserId)
        {
            var comment =await _commentRepo.GetByIdAsync(CommentId);
            if(comment.Success == false)
            {
                return new ResponseData<CommentDto>
                {
                    Message = "Not found !"
                };
            }

            if(comment.Data.CreatedBy != UserId)
            {
                return new ResponseData<CommentDto> { Message = " You Cannot Delete the Comment" };
            }

            var result = await _commentRepo.DeleteAsync(comment.Data);
            await _unitOfWork.CommitAsync();
            return new ResponseData<CommentDto>
            {
                Success = true,
                Message = result.Message
            };

            
        }


        public async Task<ResponseData<CommentDto>> GetCommentByIdAsync(Guid CommentId)
        {
            var comment = await _commentRepo.GetByIdAsync(CommentId);
            if (comment.Success == true)
            {
                var commentDto = new CommentDto
                {
                    Id = comment.Data.Id,
                    MovieId = comment.Data.MovieId,
                    Content = comment.Data.Content,
                    CreatedBy = comment.Data.CreatedBy,
                    CreatedAt = comment.Data.CreatedAt
                };
                return new ResponseData<CommentDto>
                {
                    Success = true,
                    Message = "Comment retrieved successfully",
                    Data = commentDto
                };
            }
            else
            {
                return new ResponseData<CommentDto>
                {
                    Success = false,
                    Message = "Comment not found",
                    Data = null
                };
            }
        }




        public async Task<ResponseData<PageResult<CommentDto>>> GetCommentsByMovieAsync(Guid MovieId,int pageNumber, int pageSize)
        {
           var data = await _repo.GetAllComment(MovieId,pageNumber,pageSize);
            if (data.Success == true)
            {
                var comments = new PageResult<CommentDto>
                {
                    Items = data.Data.Items.Select(c => new CommentDto
                    {
                        Id = c.Id,
                        MovieId = c.MovieId,
                        Content = c.Content,
                        CreatedBy = c.CreatedBy,
                        CreatedAt = c.CreatedAt,
                        UpdatedBy = c.UpdatedBy,
                        UpdatedAt = c.UpdatedAt

                    }).ToList(),
                    TotalItems = data.Data.TotalItems,
                    PageNumber = data.Data.PageNumber,
                    PageSize = data.Data.PageSize
                };
                return new ResponseData<PageResult<CommentDto>>
                {
                    Success = true,
                    Message = comments.Items.Any() ? "Comments retrieved successfully" : "No comments found",
                    Data = comments
                };
            }
                return new ResponseData<PageResult<CommentDto>>
                {
                    Success = false,
                    Message = "No comments found",
                    Data = null
                };

         }



        public async Task<ResponseData<CommentDto>> UpdateMovieCommentAsync(UpdateCommentDto dto, string UserId)
        {
            var result = await _commentRepo.GetByIdAsync(dto.Id);
            if (result.Success == false)
            {
                return new ResponseData<CommentDto> {Success=false, Message = "Comment not found"};
            }

            if (result.Data.CreatedBy != UserId)
            {
                return new ResponseData<CommentDto>
                {
                    Success = false,
                    Message = "You are not Authorize to Update"
                };
            }

            var data = result.Data;
            data.UpdatedAt = DateTime.Now;
            data.UpdatedBy = UserId;
            data.Content = dto.Content;

        
               

            var response = await _commentRepo.UpdateAsync(data);
            if(response.Success == true)
            {
                await _unitOfWork.CommitAsync();

                var comment = new CommentDto
                {
                    MovieId = response.Data.MovieId,
                    Content = response.Data.Content,
                };
                return new ResponseData<CommentDto>
                {
                    Success = true,
                    Message = response.Message,
                    Data = comment
                };

            }
            return new ResponseData<CommentDto>
            {
                Success = false,
                Message = response.Message,

            };
        }


        public async Task<ResponseData<int>> GetActiveCommentCount()
        {
            var count = await _commentRepo.GetCount(x => x.IsActive);
            return new ResponseData<int>
            {
                Success = true,
                Message = "Active comment count retrieved successfully",
                Data = count
            };
        }




    }
}
