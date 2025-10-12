using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Application.Interface.Services;
using Microsoft.Extensions.Configuration;

namespace Application.Service
{
     public class ZeroBounceService : IZeroBounceService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ZeroBounceService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<ZeroBounceResponse> ValidateEmailAsync(string email)
        {
            var apiKey = _configuration["ZeroBounce:ApiKey"];
            var apiUrl = $"https://api.zerobounce.net/v2/validate?api_key={apiKey}&email={email}";
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ZeroBounceResponse>(apiUrl);
                return new ZeroBounceResponse
                {
                    Address = response.Address,
                    Status = response.Status,
                    SubStatus = response.SubStatus,
                    FreeEmail = response.FreeEmail,
                    Domain = response.Domain,
                    MXFound = response.MXFound,
                    Account = response.Account,
                    DidYouMean = response.DidYouMean,
                    DomainAgeDays = response.DomainAgeDays,
                    FirstName = response.FirstName,
                    LastName = response.LastName,
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error validating email: {ex.Message}");
                return new ZeroBounceResponse
                {
                    Address = email,
                    Status = "error",
                    SubStatus = "exception",
                    FreeEmail = false,
                    Domain = null,
                    MXFound = false
                };
            }

        }
    }
}
