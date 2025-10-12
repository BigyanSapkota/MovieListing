using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Application.DTO;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OpenAI.Chat;
using Org.BouncyCastle.Asn1.Tsp;

namespace Application.Service
{
     public class GroqService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GroqService(IConfiguration config)
        {
            _httpClient = new HttpClient();
            _apiKey = config["Groq:ApiKey"];
            _httpClient.BaseAddress = new Uri("https://api.groq.com/openai/v1/");
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _apiKey);
        }


        public async Task<string> GetChatCompletionRawAsync(string userMessage)
        {
            var request = new ChatCompletionRequest
            {
                Model = "llama-3.3-70b-versatile",
                Messages = new List<DTO.ChatMessage>
        {
            new DTO.ChatMessage { Role = "user", Content = userMessage }
        }
            };

            var json = System.Text.Json.JsonSerializer.Serialize(
                request,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
            );

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("chat/completions", content);

            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return $"Groq API returned {response.StatusCode}: {responseString}";
            }

            //return responseString; 

            using var doc = JsonDocument.Parse(responseString);
            var message = doc.RootElement
                             .GetProperty("choices")[0]
                             .GetProperty("message")
                             .GetProperty("content")
                             .GetString();

            return message;

        }




    }
}
