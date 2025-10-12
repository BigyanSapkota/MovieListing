using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interface.Repository;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Infrastructure.Repositories
{
     public class GenreRepo : IGenreRepo
    {
        private readonly ApplicationDbContext _context;
        public GenreRepo(ApplicationDbContext context)
        {
            _context = context;
        }



        public async Task<ResponseData<PageResult<Genre>>> GetAllGenre(int pageNumber, int pageSize)
        {
            var data = _context.Genre.Where(x => x.IsActive);
            var totalCount = await data.CountAsync();

            var genres = await data
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new PageResult<Genre>
            {
                Items = genres,
                TotalItems = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            
            return new ResponseData<PageResult<Genre>>
            {
                Success = true,
                Message = genres.Any() ? "Genres retrieved successfully" : "No genres found",
                Data = result
            };


        }
    }
}
