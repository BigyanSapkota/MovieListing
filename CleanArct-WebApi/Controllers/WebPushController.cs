using Application.DTO;
using Application.Interface.Services;
using Application.Service;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Shared.Helper;

namespace CleanArct_WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebPushController : ControllerBase
    {
        private readonly IPushNotificationService _pushService;

        public WebPushController(IPushNotificationService pushService)
        {
            _pushService = pushService;
        }



        [HttpPost("register")]
        public async Task<IActionResult> RegisterToken([FromBody] RegisterDeviceTokenDto dto)
        {
            var user = UserInfoHelper.GetUserId(User);
            var token = new DeviceToken
            {
                UserId = user,
                Token = dto.Token,
                Platform = dto.Platform
            };
         
             await _pushService.RegisterTokenAsync(token);
            //if (!result)
            //{
            //    return BadRequest("Token registration failed or token already exists.");
            //}
            return Ok("Token registered successfully");
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendNotification([FromBody]NotificationRequest request)
        {
            try
            {
                await _pushService.SendNotificationAsync(request.Token,request.Title, request.Body);
                return Ok("Notifications sent");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost("Multiple-Notification")]
        public async Task<IActionResult> SendNotificationMultiple([FromBody] NotificationRequestMultiple request)
        {
            try
            {
                await _pushService.SendNotificationsToMultipleUsersAsync(request.Tokens, request.Title, request.Body);
                return Ok("Notifications sent");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }




    }
}
