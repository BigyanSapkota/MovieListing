using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;

namespace Application.Interface.Services
{
    public interface IPdfService
    {
        Task<Byte[]> GeneratePdf(BillSummaryDto generateBill,string userId);
       
    }
}
