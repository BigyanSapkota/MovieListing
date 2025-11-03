using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace CleanArct_WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [Authorize]
        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            var userId = User.Identity?.Name ?? "Anonymous";
            //_logger.LogInformation("User {UserId} requested weather", userId);
            //_logger.LogInformation("This is an info log at {time}", DateTime.Now);


            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]

            })
            .ToArray();

        }



        [HttpGet("notfound")]
        public IActionResult GetNotFound()
        {
            throw new KeyNotFoundException("The requested resource was not found.");
        }

        [HttpGet("unauthorized")]
        public IActionResult GetUnauthorized()
        {
            throw new UnauthorizedAccessException("You are not authorized to access this resource.");
        }

        [HttpGet("badrequest")]
        public IActionResult GetBadRequest()
        {
            throw new ArgumentException("Invalid argument provided.");
        }

        [HttpGet("ok")]
        public IActionResult GetOk()
        {
            return Ok(new { message = "No exception, all good ?" });
        }



    }
}
