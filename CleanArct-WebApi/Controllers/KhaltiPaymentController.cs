
using Application.DTO.Payment;
using Application.Interface.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shared.Helper;

namespace CleanArct_WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KhaltiPaymentController : ControllerBase
    {

        private readonly IKhaltiPaymentService _service;
        private readonly ILogger<KhaltiPaymentController> _logger;

        public KhaltiPaymentController(IKhaltiPaymentService service, ILogger<KhaltiPaymentController> logger)
        {
            _service = service;
            _logger = logger;
        }


        [HttpPost("Khalti-Payment")]
        public async Task<IActionResult> KhaltiPaymentInitiate()
        {
            _logger.LogInformation("Khalti payment initiation requested at {Time}", DateTime.Now);
            var result = await _service.InitiatePaymentAsync();
            if (result != null)
            {
                _logger.LogInformation("Khalti payment initiation SUCCESS. Transaction Id={Pidx}", result.pidx);
                return Ok(new
                {
                    Success = true,
                    Message = "Khalti payment initiated successfully",
                    Data = result
                });
            }
            _logger.LogWarning("Khalti payment initiation FAILED at {Time}", DateTime.Now);
            return BadRequest(new
            {
                Success = true,
                Message = "Payment Failed "
            });
        }



        [HttpPost("Verify-khalti")]
        public async Task<IActionResult> VerifyKhalti(string pidx)
        {
            _logger.LogInformation("Khalti payment verification requested.");

            var result = await _service.VerifyPaymentAsync(pidx);
            if (result != null)
            {
                _logger.LogInformation("Khalti payment verification SUCCESS. Transactin Id={Pidx}, Refrence Id={TransactionId}, Amount={Amount}", pidx, result.KhaltiResponse.transaction_id, result.KhaltiResponse.total_amount);
                return Ok(new
                {
                    Success = true,
                    Message = "Khalti payment successfully",
                    Data = result
                });
            }
            _logger.LogWarning("Khalti payment verification FAILED. Transaction Id={Pidx}", pidx);
            return BadRequest(new
            {
                Success = true,
                Message = "Payment Failed  "
            });

        }



    }
}
