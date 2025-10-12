using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Application.Service
{
     public class GrokAIService
    {
        private readonly HttpClient _httpClient;
        //private readonly IConfiguration _configuration;
        private readonly string _apiKey;
        private readonly string _baseUrl;

        public GrokAIService(HttpClient httpClient,IConfiguration configuration)
        {
            _httpClient = httpClient;
            //_configuration = configuration;
            _apiKey = configuration["GrokAI:ApiKey"];
            _baseUrl = configuration["GrokAI:BaseUrl"];

            _httpClient.BaseAddress = new Uri(_baseUrl);
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _apiKey);

        }

        //    public async Task<string> GetAIResponseAsync(string input)
        //    {
        //        //var request = new
        //        //{
        //        //    prompt = input,
        //        //    max_tokens = 100
        //        //};

        //        _httpClient.DefaultRequestHeaders.Add("Authorization",$"Bearer {_apiKey}");
        //        //var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}generate", request);

        //        var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}Chat/completions", new
        //        {
        //            model = "grok-beta",  
        //            messages = new[]
        //{
        //    new { role = "system", content = "You are a helpful assistant." },
        //    new { role = "user", content = input }
        //}
        //        });


        //        response.EnsureSuccessStatusCode();
        //        var result = await response.Content.ReadFromJsonAsync<dynamic>();
        //        return result?.choices[0]?.text??"No response from AI";

        //    }



        public async Task<string> GetAIResponseAsync(string input)
        {
            var requestBody = new
            {
                model = "grok-beta",
                messages = new[]
                {
                    new { role = "system", content = "You are a helpful assistant." },
                    new { role = "user", content = input }
                }
            };

            // Post to relative path (BaseAddress handles the full URL)
            var response = await _httpClient.PostAsJsonAsync("chat/completions", requestBody);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Grok API error: {response.StatusCode} - {error}");
            }

            var content = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(content);
            var choices = doc.RootElement.GetProperty("choices");

            if (choices.GetArrayLength() == 0)
                return "No response from AI";

            var message = choices[0].GetProperty("message").GetProperty("content").GetString();
            return message ?? "No response from AI";
        }
    }


}

