using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Shared.Common;

namespace Application.Interface.Repository
{
     public interface IGenreRepo
    {
        public Task<ResponseData<PageResult<Genre>>> GetAllGenre(int pageNumber,int pageSize);
    }
}
