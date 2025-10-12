using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Application.DTO.Payment;
using Application.Interface.Services;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Shared.Helper;

namespace CleanArct_WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EsewaController : ControllerBase
    {
        private readonly IEsewaPaymentService _esewaService;
        private readonly ILogger<EsewaController> _logger;
        public EsewaController(IEsewaPaymentService esewaService, ILogger<EsewaController> logger)
        {
            _esewaService = esewaService;
            _logger = logger;
        }


        [HttpPost("initiate")]
        public async Task<IActionResult> InitiatePayment()
        {
            _logger.LogInformation("Initiating Esewa payment at {Time}", DateTime.Now);
            var response = await _esewaService.InitiatePaymentAsync();
            if(response != null)
            {
                _logger.LogInformation("Esewa Payment initiation SUCCESS: {@Response}", response);
                return Ok(response);
            }
            _logger.LogWarning("Esewa Payment initiation FAILED ");
            return BadRequest("Error !");
        }



        [HttpGet("verify-status")]
        public async Task<IActionResult> PaymentStatus(string data)
        {
            _logger.LogInformation("Verifying Esewa payment status at {Time}", DateTime.Now);
            var response = await _esewaService.VerifyPaymentAsync(data);
            if (response != null)
            {
                _logger.LogInformation("Esewa Payment verification SUCCESS: {@Response}", response);
                return Ok(response);
            }
            _logger.LogWarning("Esewa Payment verification FAILED ");
            return BadRequest("Error !");
        }





    }

    }

