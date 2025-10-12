using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO.Movie;
using Application.Interface.Repository;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Common;

namespace Infrastructure.Repositories
{
     public class MovieCommentRepo : IMovieCommentRepo
    {
        private readonly ApplicationDbContext _context;
        public MovieCommentRepo(ApplicationDbContext context)
        {
            _context = context;
        }



        public async Task<ResponseData<PageResult<Comment>>> GetAllComment(Guid movieId,int pageNumber, int pageSize)
        {
            var data = _context.Comment.Where(x => x.IsActive && x.MovieId == movieId);
            var totalCount = await data.CountAsync();

            var comments = await data.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            var result = new PageResult<Comment>
            {
                Items = comments,
                TotalItems = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return new ResponseData<PageResult<Comment>>
            {
                Success = true,
                Message = comments.Any() ? "Comments retrieved successfully" : "No comments found",
                Data = result
            };
        }




        public async Task<ResponseData<List<CommentDto>>> GetCommentByMovieAsync(Guid movieId)
        {
            var comments = await _context.Comment
                .Where(x => x.MovieId == movieId && x.IsActive)
                .Select(x => new CommentDto
                {
                    Id = x.Id,
                    MovieId = x.MovieId,
                    Content = x.Content,
                    CreatedBy = x.CreatedBy,
                    CreatedAt = x.CreatedAt,
                    UpdatedBy = x.UpdatedBy,
                    UpdatedAt = x.UpdatedAt,
                    DeletedBy = x.DeletedBy,
                    DeletedAt = x.DeletedAt

                })
                .ToListAsync();

            if (comments.Count == 0 || comments == null)
            {
                return new ResponseData<List<CommentDto>>
                {
                    Success = false,
                    Message = "No comments found",
                    Data = null
                };
            }

            return new ResponseData<List<CommentDto>>
            {
                Success = true,
                Message = "List of comments",
                Data = comments
            };
        }




    }
}
