using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Domain.Entities;

namespace Application.Interface.Services
{
     public interface IBillService
    {
        Task<IEnumerable<BillDTO>> GetAllAsync();
        Task<BillDTO> GetByIdAsync(Guid id);
        Task<IEnumerable<BillDTO>> GetByUserIdAsync(string userId);
        Task<Bill> CreateAsync(CreateBill billDto);
        Task<Bill> UpdateAsync(BillDTO billDto);
        Task<Guid> DeleteAsync(Guid id);
        Task<BillSummaryDto> GenerateNextBillsAsync(string userId);
        Task<IEnumerable<CreateBill>> GetUnpaidUserBill(string userId);
    }
}
