using System.Net.Http.Headers;
using System.Text;
using Application.DTO;
using Application.Service;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OpenAI;
using OpenAI.Chat;

namespace CleanArct_WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroqController : ControllerBase
    {
        private readonly GroqService _groqService;
        public GroqController(GroqService groqService)
        {
            _groqService = groqService;
            
        }


        [HttpGet("ask")]
        public async Task<IActionResult> Ask([FromQuery] string prompt)
        {
            var rawResponse = await _groqService.GetChatCompletionRawAsync(prompt);
            return Ok(new
            {
                Prompt = prompt,
                Response = rawResponse
            });
        }



    }
}
