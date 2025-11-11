using System;
using Application.DTO;
using Application.Interface.Services;
using Microsoft.AspNetCore.Mvc;

namespace CleanArct_WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PdfController : ControllerBase
    {
        private readonly IPdfService _pdfService;
        private readonly IBillService _billService;
        private readonly IPdfGenerateService _pdfGenerateService;
        public PdfController(IPdfService pdfService,IBillService billService, IPdfGenerateService pdfGenerateService)
        {
            _pdfService = pdfService;
            _billService = billService;
            _pdfGenerateService = pdfGenerateService;
        }

              // Using Select.Pdf
        [HttpPost("generate-pdf")]
        public async Task<IActionResult> GeneratePdf(string userId)
        {

            var generateBill = await _billService.GenerateNextBillsAsync(userId);
            var pdf = await _pdfService.GeneratePdf(generateBill,userId);
            if (pdf != null)
            {
                return Ok("Pdf for Bill has been Generated");
            }
            return BadRequest("Error");
        } 


         //   Using QuestPdf C#
        [HttpPost("generate-pdf-existing-bills")]
        public async Task<IActionResult> GeneratePdfForExistingBills(string userId)
        {
            var generateBill = await _billService.GenerateNextBillsAsync(userId);
            var pdf = await _pdfGenerateService.GeneratePdf(generateBill, userId);
            if (pdf != null)
            {
                return File(pdf, "application/pdf", "bill.pdf");
                //return Ok("Pdf for Existing Bills has been Generated");
            }
            return BadRequest("Error");


        }

        // Using QuestPdf 
        [HttpPost("generate-pdf-bills")]
        public async Task<IActionResult> GeneratePdfForBills(string userId)
        {
            var generateBill = await _billService.GenerateNextBillsAsync(userId);
            var pdf = await _pdfGenerateService.GenerateBillAsyn(generateBill, userId);
            if (pdf != null)
            {
                return File(pdf, "application/pdf", "bill.pdf");
                //return Ok("Pdf for Existing Bills has been Generated");
            }
            return BadRequest("Error");


        }


        [HttpPost("generate-pdfnew-bills")]
        public async Task<IActionResult> GeneratePdfForNewBills(string userId)
        {
            var generateBill = await _billService.GenerateNextBillsAsync(userId);
            var pdf = await _pdfGenerateService.GenerateNewBillAsync(generateBill, userId);
            if (pdf != null)
            {
                return File(pdf, "application/pdf", "bill.pdf");
                //return Ok("Pdf for Existing Bills has been Generated");
            }
            return BadRequest("Error");


        }








    }
}
