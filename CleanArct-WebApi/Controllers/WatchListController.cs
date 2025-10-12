using System.Security.Claims;
using Application.Interface.Services;
using Microsoft.AspNetCore.Mvc;

namespace CleanArct_WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WatchListController : ControllerBase
    {
        private readonly IWatchListService _watchListService;
        public WatchListController(IWatchListService watchListService)
        {
            _watchListService = watchListService;
        }




        [HttpPost("Add-WatchList")]
        public async Task<IActionResult> Add(Guid movieId)
        {
            var response = await _watchListService.AddAsync(movieId);
            if (response != null)
            {
                return Ok(response);
            }
            return BadRequest("Cannot Added !");
        }



        [HttpDelete("Delete-WatchList")]
        public async Task<IActionResult> Remove(Guid movieId)
        {
            var response = await _watchListService.RemoveAsync(movieId);
            if (response.Success)
            {
                return Ok(response.Message);
            }
            return BadRequest(response.Message);
        }


        [HttpGet("Get-WatchList")]
        public async Task<IActionResult> GetAll()
        {
           
            var response = await _watchListService.GetAllAsync();
            if (response != null && response.Any())
            {
                return Ok(response);
            }
            return NotFound("No watchlist found for the user.");
        }


    }
}
