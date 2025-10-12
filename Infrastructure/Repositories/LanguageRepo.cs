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

namespace Infrastructure.Repositories
{
    public class LanguageRepo : ILanguageRepo
    {
        private readonly ApplicationDbContext _context;
        public LanguageRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseData<PageResult<Language>>> GetAllLanguage(int pageNumber, int pageSize)
        {
            var data = _context.Language.Where(x => x.IsActive);
            var totalCount = await data.CountAsync();

            var language = await data.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            var result = new PageResult<Language>
            {
                Items = language,
                TotalItems = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };


            return new ResponseData<PageResult<Language>>
            {
                Success = true,
                Message = language.Any() ? "Languages retrieved successfully" : "No languages found",
                Data = result
            };


        }
    }
}
