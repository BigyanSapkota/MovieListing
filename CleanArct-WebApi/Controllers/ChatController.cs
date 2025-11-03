using Application.DTO;
using Application.Interface.Services;
using Application.Service;
using MailKit;
using Microsoft.AspNetCore.Mvc;

namespace CleanArct_WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        //private readonly IGrokService _grokService;
        private readonly ClaudeService _claudeService;
        private readonly OpenAIService _openAIService;
        private readonly GrokAIService _grokAIService;
        //private readonly GroqAIService _aiService;

        public ChatController(ClaudeService claudeService, OpenAIService openAIService, GrokAIService grokAIService)
        {
            _claudeService = claudeService;
            _openAIService = openAIService;
            _grokAIService = grokAIService;
            //_aiService = aiService;
        }



        [HttpPost("chatsend")]
        public async Task<IActionResult> SendMessage([FromBody] string message)
        {
            var result = await _claudeService.SendMessageAsync(message);
            return Ok(result);
        }


        //[HttpPost("chat")]
        //public async Task<IActionResult> Post([FromBody] ChatRequest request)
        //{
        //    var reply = await _openAIService.GetResponseAsync( request.Message);
        //    return Ok(new ChatResponse { Reply = reply });
        //}


        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] ChatRequest request)
        {
            try
            {
                var reply = await _openAIService.GetResponseAsync(request.Message);
                return Ok(new ChatResponse { Reply = reply });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }



        [HttpPost("completions")]
        public async Task<IActionResult> AskGrokAI([FromBody] string input)
        {
            if(string.IsNullOrWhiteSpace(input))
            {
                return BadRequest("Input cannot be empty.");
            }

            var response = await _grokAIService.GetAIResponseAsync(input);
            return Ok(new { response });

        }


        //[HttpPost("askgroq")]
        //public async Task<IActionResult> AskAI([FromBody] AskRequest request)
        //{
        //    var result = await _aiService.GetAIResponseAsync(request.Prompt);
        //    return Ok(new { response = result });
        //}

        //public class AskRequest
        //{
        //    public string Prompt { get; set; }
        //}


    }
}
