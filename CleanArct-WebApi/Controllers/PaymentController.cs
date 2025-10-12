using Application.DTO.Payment;
using Microsoft.AspNetCore.Mvc;

namespace CleanArct_WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public PaymentController(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();
        }

        // 1️⃣ Initiate Payment (returns eSewa URL)
        [HttpPost("initiate")]
        public IActionResult InitiatePayment([FromBody] PaymentRequest request)
        {
            var merchantCode = _configuration["Esewa:MerchantCode"];
            var successUrl = _configuration["Esewa:SuccessUrl"];
            var failureUrl = _configuration["Esewa:FailureUrl"];

           var amt = request.Amount;  
           var txAmt = 0;              
            var pdc = 0;               
           var psc = 0;                
            var tAmt = amt + txAmt + psc + pdc;  
            var pid = request.OrderId;  

            var esewaUrl = $"https://esewa.com.np/epay/main?amt={amt}&txAmt={txAmt}&psc={psc}&pdc={pdc}&tAmt={tAmt}&pid={pid}&scd={merchantCode}&su={successUrl}&fu={failureUrl}";

            return Ok(new { PaymentUrl = esewaUrl });
        }

       

        [HttpGet("success")]
        public async Task<IActionResult> PaymentSuccess([FromQuery] string amt, [FromQuery] string pid, [FromQuery] string refId)
        {
            var merchantCode = _configuration["Esewa:MerchantCode"];

            var values = new Dictionary<string, string>
        {
            { "amt", amt },
            { "pid", pid },
            { "scd", merchantCode },
            { "rid", refId }
        };

            var content = new FormUrlEncodedContent(values);
            var response = await _httpClient.PostAsync("https://rc-epay.esewa.com.np/api/epay/main/v2/form", content);
            //var response = await _httpClient.PostAsync("https://esewa.com.np/epay/transrec", content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (responseString.Contains("Success"))
            {
                // ✅ Payment verified successfully
                // TODO: Update order/payment status in your DB
                return Ok(new { Message = "Payment successful and verified!", PaymentId = pid, RefId = refId });
            }

            return BadRequest(new { Message = "Payment verification failed." });
        }


        [HttpGet("failure")]
        public IActionResult PaymentFailure([FromQuery] string pid)
        {
            return BadRequest(new { Message = "Payment failed.", PaymentId = pid });
        }


    }
}
