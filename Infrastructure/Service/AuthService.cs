using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Application.Interface.Repository;
using Application.Interface.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Shared.Common;
using Shared.Helper;


namespace Application.Service
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User>? _userManager;
        private readonly SignInManager<User>? _signInManager;
        private readonly RoleManager<Role>? _roleManager;
        private readonly IConfiguration _configuration;
        private readonly EmailHelper _emailHelper;
        private readonly IPaymentRepo _paymentRepo;
        public AuthService(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<Role>? roleManager, IConfiguration configuration, EmailHelper emailHelper, IPaymentRepo paymentRepo)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _emailHelper = emailHelper;
            _paymentRepo = paymentRepo;
        }


        public async Task<ResponseData> ChangePasswordAsync(string currentUserEmail, ChangePasswordDto changePasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(currentUserEmail);
            if (user == null || user.Email != currentUserEmail)
            {
                return new ResponseData
                {
                    Success = false,
                    Message = "UnAuthorize !"
                };
            }

            var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.OldPassword, changePasswordDto.NewPassword);

            if (result.Succeeded)
            {
                return new ResponseData
                {
                    Success = true,
                    Message = "Password changed successfully."
                };
            }

            return new ResponseData
            {
                Success = false,
                Message = string.Join(", ", result.Errors.Select(e => e.Description))
            };
        }





        public async Task<ResponseData> ForgetPasswordAsync(ForgetPasswordDto forgetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(forgetPasswordDto.Email);
            if (user == null)
            {
                return new ResponseData
                {
                    Success = false,
                    Message = "User not found."
                };
            }
            //var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var otp = GenerateOtpHelper.GenerateRandomOtp();
            var expiry = DateTime.Now.AddMinutes(20);

            // save to Usertoken
            await _userManager.SetAuthenticationTokenAsync(user, "OTPProvider", "PasswordReset", otp + "|" + expiry);


            _emailHelper.SendEmail(
                     _configuration["EmailSettings:SenderName"],
                     _configuration["EmailSettings:SenderEmail"],
                      user.UserName,
                      user.Email,
                      "Password Reset",
                      $"Your OTP code for Password Reset is: {otp}. It is valid till {expiry} ."
                );


            return new ResponseData
            {
                Success = true,
                Message = "Password reset token generated successfully.",
                //Data = otp
            };

        }




        public async Task<ResponseData> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
            {
                return new ResponseData
                {
                    Success = false,
                    Message = "User not found."
                };
            }

            var reset = await _userManager.GetAuthenticationTokenAsync(user, "OTPProvider", "PasswordReset");
            if (reset == null)
            {
                return new ResponseData
                {
                    Success = false,
                    Message = "OTP not found or expired. Please request a new OTP."
                };
            }

            var parts = reset.Split('|');
            var otp = parts[0];
            var expiry = parts[1];

            if (resetPasswordDto.Otp != otp || DateTime.Now > DateTime.Parse(expiry))
            {
                return new ResponseData
                {
                    Success = false,
                    Message = "Invalid or expired OTP."
                };
            }

            // Generate Identity Reset Token
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);



            var result = await _userManager.ResetPasswordAsync(user, resetToken, resetPasswordDto.NewPassword);
            if (!result.Succeeded)
            {
                return new ResponseData
                {
                    Success = false,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                };
            }

            await _userManager.RemoveAuthenticationTokenAsync(user, "OTPProvider", "PasswordReset");
            return new ResponseData
            {
                Success = true,
                Message = "Password reset successfully."
            };

        }







        public async Task<ResponseData> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return new ResponseData
                {
                    Success = false,
                    Message = "Invalid email or password."
                };
            }

            bool passwordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!passwordValid)
            {
                return new ResponseData
                {
                    Success = false,
                    Message = "Incorrect password."
                };
            }

            var token = await GenerateJwtToken(user);

            return new ResponseData
            {
                Success = true,
                Message = "Login successful",
                Data = new TokenData
                {
                    Token = token,
                    Expiration = DateTime.Now.AddMinutes(300)
                }
            };
        }



     



        public async Task<string> GenerateJwtToken(User user)
        {

            var roles = await _userManager.GetRolesAsync(user);
            string orgName = string.Empty; 

            if (user.OrganizationId != null)
            {
                var orgId = user.OrganizationId.Value;
                var organization = await _paymentRepo.GetUserOrganizationAsync(orgId);
                orgName = organization?.Name ?? string.Empty; 
            }

            var claim = new List<Claim>
            {
               new Claim(ClaimTypes.NameIdentifier, user.Id.ToString() ?? string.Empty),
               new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
               new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim("PhoneNumber", user.PhoneNumber ?? string.Empty),
                new Claim("OrganizationId", user.OrganizationId.ToString() ?? string.Empty),
                new Claim("OrganizationName", orgName)
           };

            foreach (var role in roles)
            {
                claim.Add(new Claim(ClaimTypes.Role, role));
            }


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:ValidIssuer"],
                audience: _configuration["Jwt:ValidAudience"],
                claims: claim,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }






        public async Task<ResponseData> RegisterAsync(RegisterDto registerDto)
        {


            var user = new User
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
                //Role = registerDto.Role
            };


            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                return new ResponseData
                {
                    Success = false,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description)),
                    Data = null
                };
            }


            // Add default role
            await _userManager.AddToRoleAsync(user, "User");

            // Generate OTP
            var otp = GenerateOtpHelper.GenerateRandomOtp();
            var expiry = DateTime.Now.AddMinutes(100);

            await _userManager.SetAuthenticationTokenAsync(user, TokenOptions.DefaultProvider, "EmailConfirmation", otp + "|" + expiry);

            //var userOtp = new UserOtp
            //{
            //    userId = user.Id,
            //    OTP = otp,
            //    ExpiryTime = DateTime.Now.AddDays(100),
            //    IsUsed = false,
            //    CreatedAt = DateTime.Now
            //};

            // Save OTP to database (assuming you have a DbContext and UserOtp DbSet)
            //await _otpRepo.SaveOtpAsync(userOtp);


            // Send OTP through Email


            _emailHelper.SendEmail(
                    _configuration["EmailSettings:SenderName"],
                    _configuration["EmailSettings:SenderEmail"],
                     user.UserName,
                     user.Email,
                     "Please Verify Your Account",
                      $"Your OTP code is: {otp}. It is valid for 100 days."
               );


            return new ResponseData
            {
                Success = result.Succeeded,
                Message = result.Succeeded ? "User registered successfully.Please Verify your Account" : string.Join(", ", result.Errors.Select(e => e.Description)),
                Data = "Welcome " + user.UserName
            };


        }




        public async Task<ResponseData> VerifyOtpAsync(string email, string otp)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new ResponseData { Success = false, Message = "User Not Found" };

            }
            var userOtp = await _userManager.GetAuthenticationTokenAsync(user, TokenOptions.DefaultProvider, "EmailConfirmation");

            if (userOtp == null)
            {
                return new ResponseData
                {
                    Success = false,
                    Message = "OTP not found or expired. Please request a new OTP."
                };
            }

            var parts = userOtp.Split('|');
            var newotp = parts[0];
            var expiry = parts[1];

            if (otp != newotp || DateTime.Now > DateTime.Parse(expiry))
            {
                return new ResponseData
                {
                    Success = false,
                    Message = "Invalid or expired OTP."
                };
            }

            // Generate Identity Reset Token
            //var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);


            //var verify = await _userManager.VerifyUserTokenAsync(user, "OTPProvider", "RegisterUser", resetToken);
            //var verify = await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, "EmailConfirmation", resetToken);

            //if (!verify)
            //{
            //    return new ResponseData
            //    {
            //        Success = false,
            //        Message = "Invalid or Expired Token."
            //    };
            //}

            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);

            await _userManager.RemoveAuthenticationTokenAsync(user, TokenOptions.DefaultProvider, "RegisterUser");


            return new ResponseData
            {
                Success = true,
                Message = "User Verified"
            };

        }

        public async Task<ResponseData<List<UserDto>>> GetVerifiedUserAsync()
        {
            if (_userManager == null)
            {
                throw new InvalidOperationException("UserManager is not initialized.");
            }
            var data = await _userManager.Users.Where(u => u.EmailConfirmed).ToListAsync();

            return new ResponseData<List<UserDto>>
            {
                Success = true,
                Message = "Verified Users List",
                Data = data.Select(x => new UserDto
                {
                    UserId = x.Id,
                    UserName = x.UserName ?? string.Empty,
                    Email = x.Email ?? string.Empty
                }).ToList() 
            };
        }






    }
}
