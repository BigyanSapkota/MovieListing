using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Interface.Services;
using Microsoft.Extensions.Configuration;

namespace Application.Service
{
    public class GrokService : IGrokService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public GrokService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<string> GetCompletionAsync(string prompt)
        {
            var apiKey = _config["Grok:ApiKey"];

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey);

            var requestBody = new
            {
                messages = new object[]
                {
                    new { role = "system", content = "You are a test assistant." },
                    new { role = "user", content = prompt }
                },
                model = "grok-4-latest",
                stream = false,
                temperature = 0
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://api.x.ai/v1/chat/completions", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }
    }

}
