using Application.Interface.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArct_WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeleteRequestController : ControllerBase
    {
        private readonly IDeleteRequestService _deleteRequestService;
        public DeleteRequestController(IDeleteRequestService deleteRequestService)
        {
            _deleteRequestService = deleteRequestService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Approve-Request")]
        public async Task<IActionResult> ApproveDeleteRequest(Guid requestId)
        {            
           var result = await _deleteRequestService.ApproveDeleteRequestAsync(requestId);
            return Ok(result);
        }


        [HttpGet("Pending-Request")]
        public async Task<IActionResult> GetPendingRequest()
        {
            var result = await _deleteRequestService.GetPendingRequestsAsync();
            if(result == null || !result.Any())
            {
                return NotFound("No pending delete requests found.");
            }
            return Ok(result);
        }

    }
}
