using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO.Movie;
using Shared.Common;

namespace Application.Interface.Services
{
     public interface IMovieCommentService
    {
        public Task<ResponseData<CommentDto>> AddMovieCommentAsync(MovieCommentDto dto, string UserId);
        public Task<ResponseData<PageResult<CommentDto>>> GetCommentsByMovieAsync(Guid MovieId,int pageNumber, int pageSize);
        public Task<ResponseData<CommentDto>> UpdateMovieCommentAsync(UpdateCommentDto dto, string UserId);
        public Task<ResponseData<CommentDto>> DeleteMovieCommentAsync(Guid CommentId, string UserId);
        public Task<ResponseData<CommentDto>> GetCommentByIdAsync(Guid CommentId);
        public Task<ResponseData<int>> GetActiveCommentCount();


    }
}
