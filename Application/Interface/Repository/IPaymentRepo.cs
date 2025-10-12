using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interface.Repository
{
     public interface IPaymentRepo
    {
        Task AddAsync(PaymentTransaction payment);
        Task UpdateAsync(PaymentTransaction payment);
        Task<PaymentTransaction> GetByPidxAsync(string pidx);
        Task<Organization?> GetUserOrganizationAsync(Guid orgId);
    }
}
