using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Shared.Common;

namespace Application.Interface.Repository
{
    public interface IGenericRepo<T,TPrimaryKey> where T : BaseEntity<TPrimaryKey>
    {

        Task<ResponseData<T>> AddAsync(T entity);
        Task<ResponseData<T>> DeleteAsync(T entity);
        Task<ResponseData<T>> UpdateAsync(T entity);
        Task<ResponseData<T>> GetByIdAsync(TPrimaryKey id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<ResponseData<T>> GetByName(string name);
        Task<int> GetCount(Expression<Func<T,bool>>? condition=null);
       
    }
}
