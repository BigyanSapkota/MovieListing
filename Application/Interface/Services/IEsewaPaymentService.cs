using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO.Payment;
using Microsoft.AspNetCore.Mvc;

namespace Application.Interface.Services
{
     public interface IEsewaPaymentService
    {
        Task<PaymentResponse> InitiatePaymentAsync();
        Task<PaymentResult> VerifyPaymentAsync(string data);
    }
}
