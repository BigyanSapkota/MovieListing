using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Application.Service
{
     public class ClaudeService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly string _apiKey;
        private readonly string _baseUrl;
        public ClaudeService( IConfiguration config)
        {
            _httpClient = new HttpClient();
            _config = config;
            _apiKey = _config["ClaudeAI:ApiKey"];
            _baseUrl = _config["ClaudeAI:BaseUrl"];
        }



        public async Task<string> SendMessageAsync(string message)
        {
            var payload = new
            {
                model = "claude-2",
                messages = new[]
                {
                new { role = "user", content = message }
            }
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            //var response = await _httpClient.PostAsync($"{_baseUrl}/messages", content);
            var response = await _httpClient.PostAsync($"{_baseUrl}/chat/completions", content);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }



    }
}
