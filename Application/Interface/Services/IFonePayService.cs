using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO.Payment;

namespace Application.Interface.Services
{
     public interface IFonePayService
    {
        Task<PaymentResponse> PaymentRequest();
    }
}
