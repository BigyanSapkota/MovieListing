using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Interface.Services;
using Domain.Entities;

namespace Application.Service
{
    public class PaymentHistoryService : IPaymentHistoryService
    {
        private readonly IPaymentHistoryRepo _paymentHistoryRepo;
        public PaymentHistoryService(IPaymentHistoryRepo paymentHistoryRepo)
        {
            _paymentHistoryRepo = paymentHistoryRepo;
        }

        public Task SavePaymentAsync(Dictionary<string, string> payload, string status)
        {
            var paymentHistory = new PaymentHistory
            {
                TransactionId = payload.ContainsKey("transaction_uuid") ? payload["transaction_uuid"] : "",
                Amount = payload.ContainsKey("total_amount") ? decimal.Parse(payload["total_amount"]) : 0,
                Status = status,
                Payload = JsonSerializer.Serialize(payload)
            };

            return _paymentHistoryRepo.AddAsync(paymentHistory);
        }
    }
}
