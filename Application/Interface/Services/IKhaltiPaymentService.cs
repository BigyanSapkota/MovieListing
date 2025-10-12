using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO.Payment;
using Domain.Entities;

namespace Application.Interface.Services
{
    public interface IKhaltiPaymentService
    {
        Task<KhaltiInitiateResponse> InitiatePaymentAsync();
        Task<KhaltiPaymentResultDto> VerifyPaymentAsync(string pidx);
       
    }
}
