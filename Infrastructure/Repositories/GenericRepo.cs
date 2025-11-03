using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Application.Interface.Repository;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Common;

namespace Infrastructure.Repositories
{
    public class GenericRepo<T,TPrimaryKey> : IGenericRepo<T,TPrimaryKey> where T : BaseEntity<TPrimaryKey>
    {
        private ApplicationDbContext _context;
        //private DbSet<T> _dbSet;
        public GenericRepo(ApplicationDbContext context)
        {
            _context = context;
            //_dbSet = _context.Set<T>();
        }



        public async Task<ResponseData<T>> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);       // no save here only add to context

            //await _context.SaveChangesAsync();
            return new ResponseData<T>
            {
                Success = true,
                Message = "Entity added successfully",
                Data = entity
            };
        }


        public Task<ResponseData<T>> DeleteAsync(T entity)
        {
            _context.Attach(entity);
            entity.IsActive= false;
            _context.Entry(entity).State = EntityState.Modified;
            return Task.FromResult(new ResponseData<T>
            {
                Success = true,
                Message = "Entity deleted successfully",
                Data = entity
            });
        }


        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return _context.Set<T>().Where(x => x.IsActive).ToList();
            
        }

        public async Task<ResponseData<T>> GetByIdAsync(TPrimaryKey id) 
        {
            var entity = await _context.Set<T>().FirstOrDefaultAsync(x => x.Id.Equals(id));

            if (entity == null || !entity.IsActive)
                {
                    return new ResponseData<T>
                    {
                        Success = false,
                        Message = "Entity not found or inactive",
                        Data = null
                    };
                }
                else
                {
                    return new ResponseData<T>
                    {
                        Success = true,
                        Message = "Entity found",
                        Data = entity
                    };
                }
            //throw new NotImplementedException();
        }

        public async Task<ResponseData<T>> GetByName(string name)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseData<T>> UpdateAsync(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            return Task.FromResult(new ResponseData<T>
            {
                Success = true,
                Message = "Entity updated successfully",
                Data = entity
            });
        }
        

       public async Task<int> GetCount(Expression<Func<T,bool>>? condition = null)
        {
            if(condition != null)
            {
                return await _context.Set<T>().CountAsync(condition);
            }
            return await _context.Set<T>().CountAsync();
        }





    }
}
