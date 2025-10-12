using Application.DTO.Movie;
using Application.Interface.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArct_WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LanguageController : Controller
    {

        private readonly ILanguageService _languageService;
        public LanguageController(ILanguageService languageService)
        {
            _languageService = languageService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Add-Language")]
        public async Task<IActionResult> AddLanguage([FromForm] CreateLanguageDto dto)
        {
            var result = await _languageService.AddLanguage(dto);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest("Failed to add language");
        }


        [HttpGet("Get-Language-ById")]
        public async Task<IActionResult> GetLanguageById(Guid languageId)
        {
            var result = await _languageService.GetLanguageById(languageId);
            if (result != null)
            {
                return Ok(result.Data);
            }
            return BadRequest(result?.Message);
        }


        [HttpGet("Get-All-Languages")]
        public async Task<IActionResult> GetAllLanguages(int pageNumber = 1, int pageSize =5)
        {
            var result = await _languageService.GetAllLanguages(pageNumber,pageSize);
            if (result != null)
            {
                return Ok(result.Data);
            }
            return BadRequest(result?.Message);

        }



        [Authorize(Roles = "Admin")]
        [HttpDelete("Delete-Language")]
        public async Task<IActionResult> DeleteLanguage(Guid languageId)
        {
            var result = await _languageService.DeleteLanguage(languageId);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest("Failed to delete language");
        }



        [Authorize(Roles = "Admin")]
        [HttpPut("Update-Language")]
        public async Task<IActionResult> UpdateLanguage([FromForm]UpdateLanguageDto updateLanguageDto)
        {
            var result = await _languageService.UpdateLanguage(updateLanguageDto);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest("Failed to update language");
        }



     }
}
