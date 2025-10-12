using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Shared.Common;

namespace Application.Interface.Repository
{
     public interface ILanguageRepo
    {
        public Task<ResponseData<PageResult<Language>>> GetAllLanguage(int pageNumber, int pageSize);
    }
}
