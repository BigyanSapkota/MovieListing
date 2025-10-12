using System.Security.Claims;
using Application.DTO;
using Application.DTO.Movie;
using Application.Interface.Services;
using AutoMapper;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Common;
using Shared.Helper;

namespace CleanArct_WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovieController : ControllerBase
    {
        private readonly EmailHelper _emailHelper;
        private readonly IMovieService _movieService;
        private readonly IMapper _mapper;
        IConfiguration _configuration;
        private readonly ILogger<MovieController> _logger;

        public MovieController(IMovieService movieService, IMapper mapper, IConfiguration configuration,
                             EmailHelper emailHelper, ILogger<MovieController> logger)
        {
            _movieService = movieService;
            _mapper = mapper;
            _emailHelper = emailHelper;
            _configuration = configuration;
            _logger = logger;
        }


        [Authorize(Roles = "Admin")]
        [HttpPost("Add-Movie")]
        public async Task<IActionResult> AddMovie([FromForm] CreateMovieDto movieDto)
        {

            var loginuser = UserInfoHelper.GetUserId(User);

            var result = await _movieService.AddMovieAsync(movieDto, loginuser);
            if (result.Success || result.Data != null)
            {
              
                return Ok(result);
            }
            return BadRequest("Error Occurs when Adding");

        }


        [Authorize(Roles = ("Admin"))]
        [HttpDelete("Delete-Movie/{id}")]
        public async Task<IActionResult> DeleteMovie(Guid id)
        {
            var loginuser = UserInfoHelper.GetUserId(User);
            var response = await _movieService.DeleteMovieAsync(id, loginuser);
            if (response.Success || response.Data != null)
            {
                return Ok(response.Message);
            }
            return BadRequest("Cannot Delete");
        }



        [HttpGet("Get-All-Movies")]
        public async Task<IActionResult> GetAllMovies(int pageNumber=1,int pageSize=3)
        {
            _logger.LogInformation("GetAllMovies called with PageNumber: {PageNumber}, PageSize: {PageSize}", pageNumber, pageSize);
            var response = await _movieService.GetAllMoviesAsync(pageNumber,pageSize);
            if (response.Success == true || response.Data != null)
            {
                _logger.LogInformation("GetAllMovies succeeded.");
                return Ok(response);
            }
            _logger.LogWarning("GetAllMovies found no movies.");
            return NotFound("No Movies Found");
        }



        [HttpGet("Get-Movie-By-Id/{id}")]
        public async Task<IActionResult> GetMovieById(Guid id)
        {
            var response = await _movieService.GetMovieByIdAsync(id);
            if (response.Success || response.Data != null)
            {
                return Ok(response.Data);
            }
            return NotFound("Movie Not Found");
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("Update-Movie")]
        public async Task<IActionResult> UpdateMovie([FromForm] MovieDto movieDto)
        {
            var loginuser = UserInfoHelper.GetUserId(User);
            var result = await _movieService.UpdateMovieAsync(movieDto, loginuser);
            if (result.Success == true || result.Data != null)
            {
                return Ok(result);
            }
            return BadRequest("Error Occurs when Updating");
        }



        [HttpPost("Filter-Movies")]
        public async Task<IActionResult> GetMovieByFilter([FromQuery] MovieFilterDto filter)
        {
            var response = await _movieService.GetMovieByFilterAsync(filter);
            if (response.Success || response.Data != null)
            {
                return Ok(response.Data);
            }
            return NotFound("No Movies Found with the given filter criteria");
        }


        [Authorize]
        [HttpPost("Share-Movie/{movieId}")]
        public async Task<IActionResult> ShareMovie(Guid movieId, [FromForm] string email)
        {
            var loginUser = UserInfoHelper.GetUserName(User);
            var userEmail = UserInfoHelper.GetUserEmail(User);
            var share = await _movieService.ShareMovieAsync(movieId,email,loginUser,userEmail);
            if(share.Success == true)
            {
                return Ok(share.Message);
            }
            return BadRequest(share.Message);
        }





        [HttpPost("Send-Email")]
        public IActionResult SendnewEmail([FromBody] EmailRequestDTO request)
        {
            if (string.IsNullOrEmpty(request.ToEmail))
                return BadRequest("ToEmail cannot be empty");

            if (string.IsNullOrEmpty(request.Subject))
                return BadRequest("Subject cannot be empty");

            if (string.IsNullOrEmpty(request.Body))
                return BadRequest("Body cannot be empty");

            string senderName = _configuration["EmailSettings:SenderName"];
            string senderEmail = _configuration["EmailSettings:SenderEmail"];

            if (string.IsNullOrEmpty(senderEmail))
                return StatusCode(500, "SenderEmail is not configured");



            try
            {
                _emailHelper.SendEmail(
                     senderName,
                     senderEmail,
                     request.ToName,
                     request.ToEmail,
                     request.Subject,
                     request.Body
                    );

                return Ok("Email sent successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error sending email: {ex.Message}");


            }
        }

        //[HttpGet("Get-Active-Movie-Count")]
        //public async Task<IActionResult> GetActiveMovieCount()
        //{
        //    var result = await _movieService.GetActiveMovieCount();
        //    return Ok(result);
        //}

        //[HttpGet("Get-All-Movie-Count")]
        //public async Task<IActionResult> GetAllMovieCount()
        //{
        //    var result = await _movieService.GetAllMovieCount();
        //    return Ok(result);
        //}



        }
}
