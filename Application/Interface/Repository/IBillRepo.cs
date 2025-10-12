using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interface.Repository
{
    public interface IBillRepo
    {
       // Task<List<WaterBill>> GetUnpaidWaterBillByUserIdAsync(string userId);
       //Task<WaterBill> GeWBillByIdAsync(Guid bilId);
       // Task AddWaterAsync(WaterBill bill);
       // Task<List<ElectricityBill>> GetUnpaidElectricityBillByUserAsync(string userId);
       // Task<ElectricityBill> GetEBillByIdAsync(Guid billId);
       // Task AddElectricityAsync(ElectricityBill bill);
        Task SaveAsync();

        Task<List<Bill>> GetAllUnpaidBillAsync(string userId);
        Task<Bill> GetBillByIdAsync(Guid billId);
        Task<List<Bill>> GetAllUserBillsAsync(string userId);
        Task<Bill?> GetLatestBillAsync(string userId, Guid billTypeId);

    }
}
