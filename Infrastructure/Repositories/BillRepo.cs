using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interface.Repository;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace Infrastructure.Repositories
{
     public class BillRepo : IBillRepo
    {
        private readonly ApplicationDbContext _context;
        public BillRepo(ApplicationDbContext context)
        {
            _context = context;
        }



        public async Task SaveAsync()

        {

            await _context.SaveChangesAsync();
        }


        public async Task<List<Bill>> GetAllUnpaidBillAsync(string userId)
        {
   
           return await _context.Bills.Include(b => b.BillType).Where(x => x.UserId == userId && x.IsPaid == false  ).ToListAsync();
         
        }

        public async Task<Bill> GetBillByIdAsync(Guid billId)
        {
            return await _context.Bills.Include(b => b.BillType).Where(x => x.Id == billId).FirstOrDefaultAsync();
        }

        public async Task<List<Bill>> GetAllUserBillsAsync(string userId)
        {
           return await _context.Bills.Include(b => b.BillType).Where(x => x.UserId == userId).ToListAsync();
        }

        public async Task<Bill?> GetLatestBillAsync(string userId , Guid billTypeId)
        {
            return await _context.Bills.Where(b => b.UserId == userId && b.BillTypeId == billTypeId)
                .OrderByDescending(b => b.CreatedAt)
                .FirstOrDefaultAsync();

        }

    }
}
