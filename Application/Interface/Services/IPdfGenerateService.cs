using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;

namespace Application.Interface.Services
{
     public interface IPdfGenerateService
    {
        Task<Byte[]> GeneratePdf(BillSummaryDto generateBill, string userId);
        Task<byte[]> GenerateBillAsyn(BillSummaryDto generateBill, string userId);

        Task<byte[]> GenerateNewBillAsync(BillSummaryDto generateBill, string userId);
    }
}
