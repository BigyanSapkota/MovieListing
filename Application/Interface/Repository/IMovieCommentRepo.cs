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
     public interface IMovieCommentRepo
    {
        public Task<ResponseData<List<CommentDto>>> GetCommentByMovieAsync(Guid MovieId);
        public Task<ResponseData<PageResult<Comment>>> GetAllComment(Guid MovieId,int pageNumber, int pageSize);
    }
}
