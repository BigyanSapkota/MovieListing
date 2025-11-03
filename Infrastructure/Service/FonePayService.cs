using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Application.DTO.Payment;
using Application.Interface.Services;
using Hangfire;
using Jose;
using Microsoft.Extensions.Configuration;

namespace Application.Service
{
    public class FonePayService : IFonePayService 
    {
        private readonly IConfiguration _configuration;
        public FonePayService(IConfiguration configuration)
        {
            _configuration = configuration;
        }



        public Task<PaymentResponse> PaymentRequest()
        {
            string ReturnUrl = "https://localhost:7200/payment/return";
            string SecretKey = _configuration["FonePay:SecretKey"];
            string fonePayUrl = _configuration["FonePay:FonePayDevServer"];
            string merchantCode = _configuration["FonePay:MerchantCode"];
            string refrenceNumber = Guid.NewGuid().ToString("N");
            decimal amount = 1000;
            string currencyCode = "NPR";
            string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string r1 = "OrderRef001";
            string r2 = "UserRefABC";
            string MD = "P";


            var dataValidation = Shared.Helper.EsewaSignatureHelper.GenerateDV(merchantCode,MD,refrenceNumber,amount,currencyCode,date,r1,r2,ReturnUrl,SecretKey);

            var Payload = new Dictionary<string, string>
                {
                    { "MerchantCode", merchantCode },
                    { "RefNo", refrenceNumber },
                    { "Amount", amount.ToString() },
                    { "CurrencyCode", currencyCode },
                    { "Date", date },
                    { "R1", r1 },
                    { "R2", r2 },
                    { "MD", MD }
                };

            //var redirectUrl = $"{fonePayUrl}+" +
            //                  $"MerchantCode={merchantCode}" +
            //                  $"&RefNo={refrenceNumber}" +
            //                    $"&Amount={amount}" +
            //                    $"&CurrencyCode={currencyCode}" +
            //                    $"&Date={date}" +
            //                    $"&R1={r1}" +
            //                    $"&R2={r2}" +
            //                    $"&DV={dataValidation}" +
            //                    $"&MD={MD}";

            string url = $"{fonePayUrl}?PID={merchantCode}" +
                     $"&PRN={refrenceNumber}" +
                     $"&AMT={amount}" +
                     $"&CRN={currencyCode}" +
                     $"&DT={date}" +
                     $"&R1={r1}" +
                     $"&R2={r2}" +
                     $"&MD={MD}" +
                     $"&DV={dataValidation}" +
                     $"&RU={ReturnUrl}";

            return Task.FromResult(new PaymentResponse
            {
                Success = true,
                Message = "Redirect to FonePay",
                PaymentUrl = url,
                Payload = Payload
            });



        }







        //public async Task PaymentRequest()
        //{
        //    var fonePayUrl = _configuration["FonePay:Url"];
        //    var merchantId = _configuration["FonePay:MerchantId"];
        //    var terminalId = _configuration["FonePay:TerminalId"];
        //    var secretKey = _configuration["FonePay:SecretKey"];
        //    // Implement the logic to create a payment request to FonePay
        //    // This may include creating a payment model, signing the request, and sending it to FonePay's API
        //    await Task.CompletedTask;
        //}
    }
}
