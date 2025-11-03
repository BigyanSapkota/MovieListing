using Application.Interface.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArct_WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrganizationController : ControllerBase
    {
        private readonly IOrganizationService _service;
        public OrganizationController(IOrganizationService service)
        {
            _service = service;
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost("shouldapproveby")]
        public async Task<IActionResult> ShouldApproveBy(int approvedBy)
        {
             await _service.ShouldApproveBy(approvedBy);
            return Ok("ShouldApprovedBy added successfully" );
        }

        //[Authorize(Roles = "Admin")]
        [HttpPatch("Updateshouldapproveby")]
        public async Task<IActionResult> UpdateShouldApproveBy(int approveBy)
        {
            await _service.UpdateShouldApproveByAsync(approveBy);
            return Ok("ShouldApprovedBy updated successfully");
        }


    }
}
