using Application.DTO.Movie;
using Application.Interface.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Helper;

namespace CleanArct_WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovieRatingController : ControllerBase
    {
       private readonly IMovieRatingService _movieRatingService;
        public MovieRatingController(IMovieRatingService movieRatingService)
        {
            _movieRatingService = movieRatingService;
        }

        [Authorize]
        [HttpPost("Add-Movie-Rating")]  
        public async Task<IActionResult> AddMovieRating([FromForm] MovieRatingDto dto)
        {
            var loginUser = UserInfoHelper.GetUserId(User);
            if (string.IsNullOrEmpty(loginUser))
            {
                return Unauthorized("User not authenticated");
            }

            var response = await _movieRatingService.AddMovieRatingAsync(dto, loginUser);
            if(response != null)
            {
                return Ok(response);
            }
            return BadRequest("Failed to add movie rating");

        }

        [HttpGet("Average-Rating")]
        public async Task<IActionResult> GetRatingAverage(Guid MovieId)
        {
            var response = await _movieRatingService.GetAverageRatingAsync(MovieId);
            return Ok("Rating of Movie is :" +" "+ response);
        }

        [HttpGet("Get-All-Rating-Of-Movie")]
        public async Task<IActionResult> GetAllRatings(Guid MovieId,int pageNumber = 1, int pageSize = 5)
        {
            var response = await _movieRatingService.GetRatingByMovie(MovieId,pageNumber,pageSize);
             if(response.Success && response.Data != null)
            {
               return Ok(response.Data);
            }
             return BadRequest("No ratings found for this movie");
        }


        }
}
