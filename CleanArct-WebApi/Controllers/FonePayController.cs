using Application.Interface.Services;
using Microsoft.AspNetCore.Mvc;

namespace CleanArct_WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FonePayController : ControllerBase
    {
       private readonly IFonePayService _fonePayService;
        public FonePayController(IFonePayService fonePayService)
        {
            _fonePayService = fonePayService;
        }


        [HttpPost("initiate")]
        public IActionResult PaymentInitiate()
        {
            var result = _fonePayService.PaymentRequest();
            if(result != null)
            {
                return Ok(result);
            }
            return BadRequest("Payment initiation failed");
        }


    }
}
