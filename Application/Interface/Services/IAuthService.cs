using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Domain.Entities;
using Shared.Common;

namespace Application.Interface.Services
{
     public interface IAuthService
    {
        public Task<ResponseData> RegisterAsync(RegisterDto registerDto);
        public Task<ResponseData> VerifyOtpAsync(string email, string otp);
        public Task<ResponseData> LoginAsync(LoginDto loginDto);
        public Task<string> GenerateJwtToken(User user);
        public Task<ResponseData> ForgetPasswordAsync(ForgetPasswordDto forgetPasswordDto);
        public Task<ResponseData> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        public Task<ResponseData> ChangePasswordAsync(string currentUser, ChangePasswordDto changePasswordDto);
        public Task<ResponseData<List<UserDto>>> GetVerifiedUserAsync();

    }
}
