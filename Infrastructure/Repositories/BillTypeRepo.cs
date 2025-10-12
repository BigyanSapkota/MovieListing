using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interface.Repository;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
     public class BillTypeRepo : IBillTypeRepo
    {
        private readonly ApplicationDbContext _context;
        public BillTypeRepo(ApplicationDbContext context)
        {
            _context = context;
        }



        public async Task<List<BillType>> GetAllBillType()
        {
            return await _context.BillTypes.ToListAsync();
        }


    }
}
