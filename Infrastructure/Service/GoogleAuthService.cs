using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Application.Interface.Repository;
using Application.Interface.Services;
using AutoMapper;
using Domain.Entities;
using Google.Apis.Auth;
using Google.Apis.Drive.v3.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Org.BouncyCastle.Utilities.Collections;

namespace Application.Service
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<Domain.Entities.User> _userManager;
        private readonly IAuthService _authService;
        private readonly string? _googleUserInfoUrl;
        private readonly HttpClient _httpClient;
        private readonly IGoogleAuthRepo _authRepository;


        public GoogleAuthService(IConfiguration configuration, IAuthService authService, UserManager<Domain.Entities.User> userManager,HttpClient httpClient, IGoogleAuthRepo authRepository)
        {
            _configuration = configuration;
            _googleUserInfoUrl = _configuration["Authorization:Google:UserInfoUrl"];
            _authService = authService;
            _userManager = userManager;
            _httpClient = httpClient;
            _authRepository = authRepository;
        }



        public async Task<TokenDto> AuthenticateWithGoogleAsync(string accessToken, string idToken)
        {
            //  Validate IdToken
            var payload = await ValidateGoogleTokenAsync(idToken);

            // Get more profile data with access_token
            var userProfile = await GetGoogleUserProfileAsync(accessToken);

            // Find or create local user
            var userExists = await _userManager.FindByEmailAsync(payload.Email);
            Domain.Entities.User user;
            if (userExists == null)
            {
                var firstName = payload.GivenName ?? payload.Name.Split(' ')[0];
                var lastName = payload.FamilyName ?? payload.Name.Split(' ').Last();
                var Username = $"{firstName}_{lastName}";

                user = new Domain.Entities.User
                {
                    UserName = Username,
                    Email = payload.Email,
                    EmailConfirmed = true
                };
                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                    throw new Exception("Failed to create user");
                await _userManager.AddToRoleAsync(user, "User");
            }
            else
            {
                //var firstName = payload.GivenName ?? payload.Name.Split(' ')[0];
                //var lastName = payload.FamilyName ?? payload.Name.Split(' ').Last();
                //var Username = $"{firstName}_{lastName}";

                //userExists.UserName = Username;

                user = userExists;
            }

            var accessJwt = await _authService.GenerateJwtToken(user);

            var refreshJwt = Guid.NewGuid().ToString();


            await _authRepository.SaveRefreshTokenAsync(new RefreshToken
            {
                Token = refreshJwt,
                UserId = user.Id,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            });


            return new TokenDto
            {
                AccessToken = accessJwt,
                RefreshToken = refreshJwt
            };
        }


        private static async Task<GoogleJsonWebSignature.Payload> ValidateGoogleTokenAsync(string idToken)
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken);
            if (payload == null || string.IsNullOrEmpty(payload.Email))
                throw new UnauthorizedAccessException("Invalid Google Token");

            return payload;
        }

        private async Task<GoogleUserProfileDto> GetGoogleUserProfileAsync(string accessToken)
        {
            using var httpClient = new HttpClient();
            var response = await _httpClient.GetAsync($"userinfo?access_token={accessToken}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GoogleUserProfileDto>(content)!;
        }



        public async Task<TokenDto> RefreshTokenAsync(string refreshToken)
        {
            var storedToken = await _authRepository.GetRefreshTokenAsync(refreshToken);
            if (storedToken == null || storedToken.ExpiryDate < DateTime.UtcNow)
                throw new UnauthorizedAccessException("Invalid or expired refresh token");

            
            await _authRepository.RevokeRefreshTokenAsync(refreshToken);

            // Generate new tokens
            var user = await _userManager.FindByIdAsync(storedToken.UserId);

            var newAccessToken = await _authService.GenerateJwtToken(user);
            var newRefreshToken = Guid.NewGuid().ToString();

            await _authRepository.SaveRefreshTokenAsync(new RefreshToken
            {
                Token = newRefreshToken,
                UserId = storedToken.UserId,
                ExpiryDate = DateTime.Now.AddDays(7),
                IsRevoked = false
            });

            return new TokenDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }


    }
}
