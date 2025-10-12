using Application.DTO;
using Application.Interface.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArct_WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BillController : Controller
    {
        private readonly IBillService _billService;

        public BillController(IBillService billService)
        {
            _billService = billService;
        }


        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _billService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _billService.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost("Create")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateBill dto)
        {
            var result = await _billService.CreateAsync(dto);
            return Ok(result);
        }

        [HttpPut("Update")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] BillDTO dto)
        {
            dto.Id = id;
            var result = await _billService.UpdateAsync(dto);
            return Ok(result);
        }

        [HttpDelete("Delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _billService.DeleteAsync(id);
            return Ok(result);
        }

        [HttpPost("generate-Bill")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GenerateNextBills(string userId)
        {
            var result = await _billService.GenerateNextBillsAsync(userId);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest("Failed to generate next bills.");
            
        }


    }
}
