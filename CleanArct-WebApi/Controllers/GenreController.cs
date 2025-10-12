using Application.DTO.Movie;
using Application.Interface.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArct_WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenreController : ControllerBase
    {
        private readonly IGenreService _genreService;
        public GenreController(IGenreService genreService)
        {
            _genreService = genreService;
        }


        [Authorize(Roles = "Admin")]
        [HttpPost("AddGenre")]
        public async Task<IActionResult> AddGenre([FromForm] CreateGenreDto dto)
        {
            var response = await _genreService.AddGenre(dto);
            if(response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }


        [Authorize(Roles = "Admin")]
        [HttpPatch("Update-Genre")]
        public async Task<IActionResult> UpdateGenre(UpdateGenreDto dto)
        {
            var response = await _genreService.UpdateGenre(dto);
            if(response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response.Message);

        }




        [Authorize(Roles = "Admin")]
        [HttpDelete("Delete-Genre")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _genreService.DeleteGenre(id);
            if(response.Success)
            {
               return Ok(response.Message);
            }
            return BadRequest(response.Message);
        }



        [HttpGet("Get-By-Id")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _genreService.GetGenreById(id);
            if(result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        [HttpGet("Get-All-Genre")]
        public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 3)
        {
            var response = await _genreService.GetAllGenres(pageNumber,pageSize);
            if(response.Success)
            {
                return Ok(response.Data);
            }
            return BadRequest(response.Message);
        }




    }
}
