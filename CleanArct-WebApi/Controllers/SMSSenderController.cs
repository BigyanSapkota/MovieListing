using Application.Interface.Services;
using Application.Service;
using Microsoft.AspNetCore.Mvc;

namespace CleanArct_WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SMSSenderController : Controller
    {
        private readonly ISMSSenderService _smsSenderService;
        private readonly VonageSMSService _vonageService;
        public SMSSenderController(ISMSSenderService smsSenderService,VonageSMSService vonageService)
        {
            _smsSenderService = smsSenderService;
            _vonageService = vonageService;
        }



        [HttpPost("send-sms")]
        public async Task<IActionResult> SendSMS(string phoneNumber, string message)
        {
            var result = await _smsSenderService.SendSMSAsync(phoneNumber, message);  
            if(result != null)
            {
                return Ok("SMS sent successfully");
            }
            return BadRequest("Failed to send SMS");
        }


        [HttpPost("Send-VonageSMS")]
        public async Task<IActionResult> SendSMSAsync(string phoneNumber, string message)
        {
            var result = await _vonageService.SendSMSAsync(phoneNumber, message);
            if (result.Messages[0].Status == "0")
            {
                return Ok("SMS Send Successfully by Vonage");
            }
            return BadRequest($"Failed to send SMS: {result.Messages[0].ErrorText}");
        }
     



        [HttpGet("get-all-sms")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _smsSenderService.GetAllSMSAsync();
            if(result != null)
            {
                return Ok(result);
            }
            return BadRequest("No SMS found");
        }


        [HttpGet("get-sms-by-phone")]
        public async Task<IActionResult> GetByPhoneNumber(string phoneNumber)
        {
            var result = await _smsSenderService.GetSMSByPhoneNumberAsync(phoneNumber);
            if(result != null)
            {
                return Ok(result);
            }
            return BadRequest("No SMS found for the given phone number");
        }


        [HttpDelete("delete-sms/{MessageSid}")]
        public async Task<IActionResult> DeleteSMS(string MessageSid )
        {
            var data = await _smsSenderService.DeleteSMSAsync(MessageSid);
            if (data == true)
            {
                return Ok("SMS deleted successfully");
            }
            return BadRequest("Failed to delete SMS");
        }





    }
}
