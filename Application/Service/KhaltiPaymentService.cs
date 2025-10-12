using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Application.DTO.Payment;
using Application.Interface.Repository;
using Application.Interface.Services;
using DnsClient.Internal;
using Domain.Entities;
using Google.Apis.Drive.v3.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Shared.Helper;

namespace Application.Service
{
    public class KhaltiPaymentService : IKhaltiPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IBillRepo _billRepo;
        private readonly IPaymentRepo _paymentRepo;
        private readonly ILogger<KhaltiPaymentService> _logger;
        private readonly UserManager<Domain.Entities.User> _userManager;
        public KhaltiPaymentService(IConfiguration configuration, HttpClient httpClient, IHttpContextAccessor contextAccessor, IBillRepo billRepo, IPaymentRepo paymentRepo,ILogger<KhaltiPaymentService> logger, UserManager<Domain.Entities.User> userManager)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _billRepo = billRepo;
            _contextAccessor = contextAccessor;
            _paymentRepo = paymentRepo;
            _logger = logger;
            _userManager = userManager;
            _userManager = userManager;
        }

    
        public async Task<KhaltiInitiateResponse> InitiatePaymentAsync()
        {

            var user = _contextAccessor.HttpContext?.User;
            
            if (user == null)
            {
                _logger.LogWarning("Khalti payment initiation failed: User not found");
                throw new Exception("Cannot initialize payment");
            }

            string? userId = UserInfoHelper.GetUserId(user);
           
            _logger.LogInformation("Initiating Khalti payment for UserId={UserId}", userId);

            var userEntity = await _userManager.Users.Where(u => u.Id == userId).Include(u => u.Organization).FirstOrDefaultAsync();
            if(userEntity?.Organization == null)
            {
                _logger.LogWarning("Khalti payment initiation failed: Organization not found for UserId={UserId}", userId);
                throw new Exception("Organization not found for user");
            }


            var bills = await _billRepo.GetAllUnpaidBillAsync(userId);

            if (bills.Count == 0)
            {
                _logger.LogWarning("No unpaid bills found for UserId={UserId}", userId);
                throw new Exception("No UnPaid Bill");
            }

            decimal TotalAmount = bills.Sum(b => b.TotalAmount);
            List<Guid> BillIds = bills.Select(b => b.Id).ToList();

            _logger.LogInformation("Unpaid bills found for UserId={UserId}. TotalAmount={TotalAmount}, BillCount={BillCount}",userId, TotalAmount, bills.Count);


            var result = await SendPaymentRequestAsync(TotalAmount, BillIds, userEntity,user);
            var payment = new PaymentTransaction
            {
                UserId = userId,
                BillIds = BillIds,
                TotalAmount = TotalAmount,
                Status = "Pending",
                CreatedAt = DateTime.Now,
                Pidx = result.pidx,
                BillType = "All Bill",
                Transaction_Method = "Khalti Payment",
                OrganizationId = userEntity.OrganizationId

            };
            await _paymentRepo.AddAsync(payment);

            return result;

        }





        public async Task<KhaltiInitiateResponse> SendPaymentRequestAsync(decimal totalAmount,List<Guid> billIds , Domain.Entities.User userEntity, ClaimsPrincipal user)
        {
            _logger.LogInformation("Sending Khalti payment request for UserId={UserId}, Amount={Amount}", userEntity.Id, totalAmount);


            //string masterKey = _configuration["MasterKey"];
            //var orgId = userEntity.OrganizationId.Value.ToString("N");
            //var encryptedSecret = userEntity.Organization.SecretKey;

            //string khaltiSecretKey = EncryptDecryptHelper.Decrypt(encryptedSecret, orgId, masterKey);

            Guid orgId = UserInfoHelper.GetOrganizationId(user);
            string orgName = UserInfoHelper.GetOrganizationName(user);


            string khaltiSecretKey = GetKhaltiSecretKey(orgName);
            string baseUrl = GetKhaltiBaseUrl();


            var url = $"{baseUrl}/api/v2/epayment/initiate/";

                var payload = new
                {
                    //return_url = $"{khalti.RedirectUrl}/KhaltiPayment/Success",
                    //website_url = khalti.RedirectUrl,

                    return_url = "https://localhost:5001/api/khalti/success",
                    website_url = "https://localhost:5001",

                    amount = totalAmount*100,
                    purchase_order_id = Guid.NewGuid().ToString(),
                    purchase_order_name = "Bill Payment",
                    BillIds = billIds,
                    User = userEntity.Id
                };

                var jsonPayload = JsonConvert.SerializeObject(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                //var client = new HttpClient();
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Key {khaltiSecretKey}");

                var response = await _httpClient.PostAsync(url, content);

                var responseContent = await response.Content.ReadAsStringAsync();


            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to initiate Khalti payment for UserId={UserId}. ", userEntity.Id);
                throw new HttpRequestException($"Failed to initiate Khalti payment: {responseContent}");
            }

            // Deserialize Khalti response
            var responseObj = JsonConvert.DeserializeObject<KhaltiInitiateResponse>(responseContent);

                return responseObj;
       }

        




        public async Task<KhaltiPaymentResultDto> VerifyPaymentAsync(string pidx)
        {
            _logger.LogInformation("Verifying Khalti payment. Transaction Id={Pidx}", pidx);
            //var khaltiSecretKey = _configuration["Khalti:SecretKey"];

            var find = await _paymentRepo.GetByPidxAsync(pidx);

            var userId = find.UserId;


            var userEntity = await _userManager.Users.Where(u => u.Id == userId).Include(u => u.Organization).FirstOrDefaultAsync();

            //string masterKey = _configuration["MasterKey"];
            //var orgId = userEntity.OrganizationId.Value.ToString("N");
            //var encryptedSecret = userEntity.Organization.SecretKey;

            //string khaltiSecretKey = EncryptDecryptHelper.Decrypt(encryptedSecret, orgId, masterKey);

            string orgName = userEntity.Organization.Name;
            string khaltiSecretKey = GetKhaltiSecretKey(orgName);
            string baseUrl = GetKhaltiBaseUrl();


            //var baseUrl = _configuration["Khalti:BaseUrl"];

            var url = $"{baseUrl}/api/v2/epayment/lookup/";


                var payload = new { pidx = pidx };
                var jsonPayload = JsonConvert.SerializeObject(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Key {khaltiSecretKey}");

                var response = await _httpClient.PostAsync(url, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                var khaltiResponse = JsonConvert.DeserializeObject<KhaltiVerifyResponse>(responseContent);





            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to verify Khalti payment.");
                throw new HttpRequestException($"Failed to verify Khalti payment: {responseContent}");
            }

            var responseObj = JsonConvert.DeserializeObject<KhaltiVerifyResponse>(responseContent);


            var payment = await _paymentRepo.GetByPidxAsync(pidx);
            if (payment == null)
            {
                _logger.LogError("Payment record not found for Transactio Id={Pidx}", pidx);
                throw new Exception("Payment record not found.");
            }


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
            payment.TransactionId = responseObj.transaction_id;
            payment.CompletedAt = DateTime.Now;


            await _paymentRepo.UpdateAsync(payment);

            _logger.LogInformation("Khalti payment completed successfully. Transaction Id={Pidx}, Refrence Id={TransactionId}, Amount={Amount}", pidx, responseObj.transaction_id, payment.TotalAmount);

            return new KhaltiPaymentResultDto
            {
                KhaltiResponse = responseObj,
                Bill_detail = BillDetails
            };

        }




            public string GetKhaltiSecretKey(string orgName)
        {
            return orgName switch
            {
                "Tech Corp" => _configuration["Khalti:Tech-Corp_SecretKey"],
                "Edu Solutions" => _configuration["Khalti:Edu-Solutions_SecretKey"],
                _ => throw new Exception($"Khalti secret not configured for organization '{orgName}'")
            };
        }


        public string GetKhaltiBaseUrl()
        {
            return _configuration["Khalti:BaseUrl"];
        }










    }

}
