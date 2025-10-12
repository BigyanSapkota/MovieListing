using System.Security.Claims;
using Application.DTO;
using Application.Interface.Services;
using Azure;
using CleanArct_WebApi.ViewModel;
using Domain.Entities;
using Hangfire;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Tls;
using Shared.Common;
using Shared.Helper;

namespace CleanArct_WebApi.Controllers
{

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
    {
       private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;
        private readonly IBackgroundJobClient _backgroundJobClient;
        public AuthController(IAuthService authService,IBackgroundJobClient backgroundJobClient,IConfiguration configuration)
        {
            _authService = authService;
            _backgroundJobClient = backgroundJobClient;
            _configuration = configuration;
        }



        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm]RegisterVM model)
        {
            
            var registerDto = new RegisterDto
            {
               
                UserName = model.UserName,
                Email = model.Email,
                Password = model.Password,
                PhoneNumber = model.PhoneNumber,
                Role = "User"
            };
            var response = await _authService.RegisterAsync(registerDto);
           
            if (response.Success)
            {
                return Ok(response.Message);
            }
            return BadRequest(response.Message);
        }


        [HttpPost("Verify-Otp")]    
        public async Task<IActionResult> VerifyOtpAsync(string email, string otp)
        {
            var verotp = await _authService.VerifyOtpAsync(email, otp);
            var sender = _configuration["EmailSettings:SenderName"];
            var senderEmail = _configuration["EmailSettings:SenderEmail"];

            var subject = $"Account Verification";
            var body = $"Your account has been Verified Successfully.\n"+ $"Thank you!";
            var bodyfail =  $"Your account cannot Verified. Please Try Again\n" + $"Thank you!";



            if (verotp.Success == true)
            {
                _backgroundJobClient.Enqueue<IEmailService>(x => x.SendVerificationSuccessEmail(sender,senderEmail,email,email,subject,body));
                return Ok(verotp.Message);
            }
            _backgroundJobClient.Enqueue<IEmailService>(x => x.SendVerificationFailedEmail(sender, senderEmail, email, email, subject, bodyfail));
            return BadRequest(verotp.Message);
        }






        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginVM model)
        {
            var loginDto = new LoginDto
            {
               Email = model.Email,
               Password = model.Password

            };
            var token = await _authService.LoginAsync(loginDto);
            if (token == null || !token.Success)
            {
                return Unauthorized(new { message = token?.Message ?? "Login failed" });
            }

            var tokenData = token.Data as TokenData;
            if (tokenData == null)
            {
                return Unauthorized(new { message = "Invalid token data" });
            }

            var result = new
            {
                token = tokenData.Token,
                expiration = tokenData.Expiration
            };

            return Ok(result);


        }

        [Authorize]
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
        {
            var changePasswordDto = new ChangePasswordDto
            {
                OldPassword = model.OldPassword,
                NewPassword = model.NewPassword,
                ConfirmPassword = model.ConfirmPassword
            };

            var currentUser = UserInfoHelper.GetUserEmail(User);

            var result = await _authService.ChangePasswordAsync(currentUser, changePasswordDto);
            return Ok(result);
        
        }



        [HttpPost("Forget-Password")]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordVM model)
        {
            var forgetPasswordDto = new ForgetPasswordDto
            {
                Email = model.Email
            };

            var result = await _authService.ForgetPasswordAsync(forgetPasswordDto);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }





        [HttpPost("Reset-Password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            var resetPasswordDto = new ResetPasswordDto
            {
                Email = model.Email,
                Otp = model.Otp,
                NewPassword = model.NewPassword

            };

            var result = await _authService.ResetPasswordAsync(resetPasswordDto);
            return Ok(result);
        }

        [HttpGet("GetVerifiedUser")]
        public async Task<IActionResult> GetVerifiedUsers()
        {
            var response = await _authService.GetVerifiedUserAsync();
            if (response.Success)
            {
                return Ok(response.Data);
            }
            return BadRequest(response.Message);
        }



        [HttpGet("GetUserInfo")]
        public async Task<IActionResult> GetUserInfo()
        {
            var user = User;
            if (user == null)
            {
                return Unauthorized();
            }
            var userInfo = new
            {
                Name = UserInfoHelper.GetUserName(user),
                UserId = UserInfoHelper.GetUserId(user),
                Email = UserInfoHelper.GetUserEmail(user),
                Role = UserInfoHelper.GetUserRole(user),
                Phone = UserInfoHelper.GetUserPhone(user),
                OrganizationId = UserInfoHelper.GetOrganizationId(user),
                OrganizationName = UserInfoHelper.GetOrganizationName(user)
            };
            return Ok(userInfo);
        }



     





    }
}
