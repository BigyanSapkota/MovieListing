using Application.DTO;
using Application.Interface.Services;
using Application.Service;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace CleanArct_WebApi.Controllers
{
    public class AuthGoogleController : ControllerBase
    {
        private readonly IGoogleAuthService _googleAuthService;
        public AuthGoogleController(IGoogleAuthService googleAuthService)
        {
            _googleAuthService = googleAuthService;
        }




        [HttpPost("google-signin")]
        public async Task<IActionResult> GoogleSignIn([FromBody] GoogleSignInRequest request)
        {
            var tokenDto = await _googleAuthService.AuthenticateWithGoogleAsync(request.AccessToken, request.IdToken);
            return Ok(tokenDto);
        }


        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(string refreshToken)
        {
            var result = await _googleAuthService.RefreshTokenAsync(refreshToken);
            return Ok(result);
        }


    }
}
