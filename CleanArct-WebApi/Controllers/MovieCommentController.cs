using Application.DTO.Movie;
using Application.Interface.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Helper;

namespace CleanArct_WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovieCommentController : ControllerBase
    {
        private readonly IMovieCommentService _movieCommentService;
        public MovieCommentController(IMovieCommentService movieCommentService)
        {
            _movieCommentService = movieCommentService;
        }



        [Authorize]
        [HttpPost("Add-Comment")]
        public async Task<IActionResult> AddComment([FromForm] MovieCommentDto dto)
        {
            var loginUser = UserInfoHelper.GetUserId(User);
            var response = await _movieCommentService.AddMovieCommentAsync(dto, loginUser);
            if (response.Success == true)
            {
                return Ok(response.Data);
            }
            else
            {
                return BadRequest(response.Message);

            }
        }

        [Authorize]
        [HttpDelete("Delete-Comment")]
        public async Task<IActionResult> DeleteComment(Guid commentId)
        {
            var loginUser = UserInfoHelper.GetUserId(User);
            var response = await _movieCommentService.DeleteMovieCommentAsync(commentId, loginUser);

            if(response.Success == true)
            {
                return Ok(response.Message);
            }
            return BadRequest(response.Message);

        }

        [Authorize]
        [HttpGet("Get-Comment-ById")]
        public async Task<IActionResult> GetCommentById(Guid commentId)
        {
            var response = await _movieCommentService.GetCommentByIdAsync(commentId);

            if (response.Success == true)
            {
                return Ok(response.Data);
            }
            return BadRequest(response.Message);
        }

        [Authorize]
        [HttpPut("Update-Comment")]
        public async Task<IActionResult> UpdateComment([FromForm] UpdateCommentDto dto)
        {
            var loginUser = UserInfoHelper.GetUserId(User);
            var response = await _movieCommentService.UpdateMovieCommentAsync(dto, loginUser);
            if(response.Success == true)
            {
                return Ok(response.Message);
            }
            return BadRequest(response.Message);
        }


        [HttpGet("Get-Comment-Movie")]
        public async Task<IActionResult> GetCommentByMovie(Guid MovieId,int pageNumber = 1, int pageSize = 5)
        {
            var response = await _movieCommentService.GetCommentsByMovieAsync(MovieId,pageNumber,pageSize);
            if(response.Success == true)
            {
                return Ok(response.Data);
            }
            return BadRequest(response.Message);
        }


    }
}
