using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Application.Interface.Repository;
using Domain.Entities;
using Infrastructure.Data;

namespace Infrastructure.Repositories
{
     public class PaymentRepo : IPaymentRepo
    {
        private readonly ApplicationDbContext _context;

        public PaymentRepo(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<Organization?> GetUserOrganizationAsync(Guid orgId)
        {
            return await _context.Organizations.Where(o => o.Id == orgId).FirstOrDefaultAsync();
        }



        public async Task AddAsync(PaymentTransaction payment)
        {
            _context.PaymentTransaction.Add(payment);
            await _context.SaveChangesAsync();
        }

        public async Task<PaymentTransaction> GetByPidxAsync(string pidx)
        {
            return await _context.PaymentTransaction.Where(p => p.Pidx == pidx).FirstOrDefaultAsync();
        }

        

        public async Task UpdateAsync(PaymentTransaction payment)
        {
            _context.PaymentTransaction.Update(payment);
            await _context.SaveChangesAsync();
        }
    }
}
