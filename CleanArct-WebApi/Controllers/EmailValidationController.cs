using Application.DTO;
using Application.Interface.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.Helper;

namespace CleanArct_WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailValidationController : ControllerBase
    {
        private readonly IZeroBounceService _zeroBounceService;
        private readonly ValidateEmail _validateEmail;
        public EmailValidationController(IZeroBounceService zeroBounceService, ValidateEmail validateEmail)
        {
            _zeroBounceService = zeroBounceService;
            _validateEmail = validateEmail;
        }

        [HttpPost("validate-Email")]
        public async Task<IActionResult> ValidateEmail([FromForm] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email is required.");
            }
            var result = await _zeroBounceService.ValidateEmailAsync(email);
            if(result != null )
            {
                if(result.Status == "error")
                {
                    return StatusCode(500, "Error validating email.");
                }
                return Ok(result);
            }
            return BadRequest("Something Went Wrong !");
        }



        [HttpPost("validate")]
        public async Task<IActionResult> ValidateEmailAsync([FromForm] EmailRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email))
                return BadRequest(new { success = false, message = "Email is required" });

            //bool isValid = await ValidateEmail.IsRealEmailAsync(request.Email);
            bool isValid = await _validateEmail.IsRealEmailAsync(request.Email);

            return Ok(new { success = isValid, email = request.Email });
        }


    }
}
