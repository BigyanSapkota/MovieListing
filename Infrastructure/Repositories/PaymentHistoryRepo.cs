using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interface.Services;
using Domain.Entities;
using Infrastructure.Data;

namespace Infrastructure.Repositories
{
     public class PaymentHistoryRepo : IPaymentHistoryRepo
    {
        private readonly ApplicationDbContext _context;
        public PaymentHistoryRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task AddAsync(PaymentHistory paymentHistory)
        {
            //_context.PaymentHistory.Add(paymentHistory);
            return _context.SaveChangesAsync();
        }
    }
}
