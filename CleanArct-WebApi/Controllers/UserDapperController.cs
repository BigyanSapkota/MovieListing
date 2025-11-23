using Application.Interface.Services;
using Microsoft.AspNetCore.Mvc;

namespace CleanArct_WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserDapperController : ControllerBase
    {
      private readonly IUserDapperService _userDapperService;
        public UserDapperController(IUserDapperService userDapperService)
        {
            _userDapperService = userDapperService;
        }


        [HttpGet("AllUser")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userDapperService.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("UserById")]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _userDapperService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }



    }
}
