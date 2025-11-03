using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO.Payment;
using Application.Interface.Repository;
using Application.Interface.Services;
using DnsClient.Internal;
using Domain.Entities;
using Google.Apis.Drive.v3.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using Shared.Helper;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.Service
{
     public class EsewaPaymentService : IEsewaPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IBillRepo _billRepo;
        private readonly IPaymentRepo _paymentRepo;
        private readonly ILogger<EsewaPaymentService> _logger;
        private readonly UserManager<Domain.Entities.User> _userManager;

        public EsewaPaymentService(IConfiguration configuration, HttpClient httpClient, IHttpContextAccessor contextAccessor, IBillRepo billRepo, IPaymentRepo paymentRepo,ILogger<EsewaPaymentService> logger, UserManager<Domain.Entities.User> userManager)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _contextAccessor = contextAccessor;
            _billRepo = billRepo;
            _paymentRepo = paymentRepo;
            _logger = logger;
            _userManager = userManager;
        }



        public async Task<PaymentResponse> InitiatePaymentAsync()
        {
            var user = _contextAccessor.HttpContext?.User;
            if (user == null)
            {
                _logger.LogWarning("Esewa payment initiation failed: User not found");
                throw new Exception("Cannot initialize payment");
            }

            string? userId = UserInfoHelper.GetUserId(user);
            

            _logger.LogInformation("Initiating payment for UserId={UserId} at {Time}", userId, DateTime.Now);

            var userEntity = await _userManager.Users.Where(u => u.Id == userId).Include(u => u.Organization).FirstOrDefaultAsync();
            if (userEntity?.Organization == null)
            {
                _logger.LogWarning("Esewa payment initiation failed: Organization not found for UserId={UserId}", userId);
                throw new Exception("Organization not found for user");
            }

            string orgName = UserInfoHelper.GetOrganizationName(user) ?? string.Empty;
            Guid orgId = UserInfoHelper.GetOrganizationId(user);
            string secretKey = GetEsewaSecretKey(orgName);
            string MerchantCode = GetEsewaMerchant(orgName);


            var bills = await _billRepo.GetAllUnpaidBillAsync(userId);

            if (bills.Count == 0)
            {
                _logger.LogWarning("Payment initiation aborted: No unpaid bills for UserId={UserId}", userId);
                throw new Exception("No UnPaid Bill");
            }

            decimal taxAmount = 0;
            decimal serviceCharge = 0;
            decimal deliveryCharge = 0;
            decimal Amount = bills.Sum(b => b.TotalAmount);
            List<Guid> BillIds = bills.Select(b => b.Id).ToList();

            decimal totalAmount = Amount + taxAmount + serviceCharge + deliveryCharge;



            //var totalAmount = request.Amount;
            var transactionId = Guid.NewGuid().ToString();

            var successUrl = _configuration["Esewa:SuccessUrl"];
            var failureUrl = _configuration["Esewa:FailureUrl"];
            var esewaUrl = _configuration["Esewa:EsewaUrl"];
            

            //var successUrl = "https://localhost:5001/api/payment/success";
            //var failureUrl = "https://localhost:5001/api/payment/failure";

            var payload = new Dictionary<string, string>
            {
                { "amount", Amount.ToString() },
                { "tax_amount", taxAmount.ToString() },
                { "product_service_charge", serviceCharge.ToString() },
                { "product_delivery_charge", deliveryCharge.ToString() },
                { "total_amount", totalAmount.ToString() },   
                { "transaction_uuid", transactionId },
                { "product_code", "EPAYTEST" },
                { "success_url", successUrl },
                { "failure_url", failureUrl },
                { "signed_field_names", "total_amount,transaction_uuid,product_code" }

            };

            // Generate signature
            var signature = EsewaSignatureHelper.GenerateSignature(payload["signed_field_names"],payload,secretKey);
            payload["signature"] = signature;

            var payment = new PaymentTransaction
            {
                UserId = userId,
                BillIds = BillIds,
                TotalAmount = totalAmount,
                Status = "Pending",
                CreatedAt = DateTime.Now,
                Pidx = transactionId,
                BillType = "All Bill",
                Transaction_Method = "Esewa Payment",
                OrganizationId = orgId


            };
            await _paymentRepo.AddAsync(payment);


            return new PaymentResponse
            {
                Success = true,
                Message = "Redirect user to ESewa",
                PaymentUrl = esewaUrl,
                Payload = payload  
            };
        }

        public async Task<PaymentResult> VerifyPaymentAsync(string data)
        {
            var user = _contextAccessor.HttpContext?.User;

            if (user == null)
            {
                _logger.LogWarning("Esewa payment verification failed: User not found");
                throw new Exception("Cannot verify payment");
            }

            string? userId = UserInfoHelper.GetUserId(user);

            string orgName = UserInfoHelper.GetOrganizationName(user) ?? string.Empty;

            string secretKey = GetEsewaSecretKey(orgName);
            string MerchantCode = GetEsewaMerchant(orgName);

            //var secretKey = _configuration["Esewa:SecretKey"];
            var lookupUrl = "https://rc.esewa.com.np/api/epay/transaction/status/";              // replace with live in production

            // Decode the Base64 (data)
            var json = Encoding.UTF8.GetString(Convert.FromBase64String(data));


            var response = JsonConvert.DeserializeObject<Dictionary<string,string>>(json);

            if (response == null)
                throw new Exception("Invalid payment data.");

            var signedFields = response["signed_field_names"];
            var providedSignature = response["signature"];
            var transactionUuid = response["transaction_uuid"];
            _logger.LogInformation("Checking payment status for TransactionUuid={TransactionUuid}", transactionUuid);



            // Verify Signature
            bool isValid = EsewaSignatureHelper.VerifySignature(signedFields,response, providedSignature, secretKey);
            if (!isValid)
            {
                _logger.LogError("Signature verification failed for TransactionUuid={TransactionUuid}", transactionUuid);
                throw new Exception("Invalid signature. Possible tampering detected.");
            }


            // Check payment status
            if (response["status"] == "COMPLETE")
            {
                _logger.LogInformation("Payment completed for TransactionUuid={TransactionUuid}, Amount={Amount}",transactionUuid, response["total_amount"]);
                var payment = await _paymentRepo.GetByPidxAsync(transactionUuid);
                if (payment == null)
                    throw new Exception("Payment record not found.");

                var BillDetails = new List<BillDetails>();

                foreach (var billId in payment.BillIds)
                {
                    var bill = await _billRepo.GetBillByIdAsync(billId);
                    if (bill != null)
                    {
                        bill.IsPaid = true;
                        bill.PaidAt = DateTime.Now;

                        BillDetails.Add(new BillDetails
                        {
                            BillId = bill.Id,
                            BillType = bill.BillType.Name,
                            Amount = bill.TotalAmount
                        });

                    }
                }


                payment.Status = "Completed";
                payment.TransactionId = response["transaction_code"];
                payment.CompletedAt = DateTime.Now;


                await _paymentRepo.UpdateAsync(payment);

                return new PaymentResult
                {
                    Success = true,
                    ReferenceId = response["transaction_code"],
                    Amount = Convert.ToDecimal(response["total_amount"]),
                    Transaction_Uuid = response["transaction_uuid"],
                    ProductCode = response["product_code"],
                    Message = "Payment successfull",
                    Bill_Details = BillDetails
                    

                };

            }

            return new PaymentResult
            {
                Success = false,
                Message = response["status"]
            };
         

        }



  

        public string GetEsewaSecretKey(string orgName)
        {
            return orgName switch
            {
                "Tech Corp" => _configuration["Esewa:Tech-Corp_SecretKey"],
                "Edu Solutions" => _configuration["Esewa:Edu-Solutions_SecretKey"],
                _ => throw new Exception($"Esewa secret not configured for organization '{orgName}'")
            };
        }


        public string GetEsewaMerchant(string orgName)
        {
            return orgName switch
            {
                "Tech Corp" => _configuration["Esewa:Tech-Corp_MerchantCode"],
                "Edu Solutions" => _configuration["Esewa:Edu-Solutions_MerchantCode"],
                _ => throw new Exception($"Esewa Merchant not configured for organization '{orgName}'")
            };
        }




    }
}
