using Application.DTO;
using Application.Interface;
using Application.Interface.Services;
using Domain.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;



namespace Infrastructure.Service
{
    public class PdfService : IPdfService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly Microsoft.AspNetCore.Identity.UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public PdfService(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor contextAccessor, Microsoft.AspNetCore.Identity.UserManager<User> userManager, IUnitOfWork unitOfWork)
        {
            _webHostEnvironment = webHostEnvironment;
            _contextAccessor = contextAccessor;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<byte[]> GeneratePdf(BillSummaryDto generateBill, string userId)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                string _templatePath = Path.Combine(_webHostEnvironment.WebRootPath, "Templates", "BillTemplate.html");
                string htmlTemplate = await File.ReadAllTextAsync(_templatePath);

                
                var user = await _userManager.Users.Include(u => u.Organization).FirstOrDefaultAsync(x => x.Id == userId);
                if (user == null)
                    throw new ArgumentException("User Not Found !");
                var userName = user.UserName;
                var organization = user.Organization?.Name;
                var Email = user.Email;
                var Contact = user.PhoneNumber;

                decimal totalPrevious = generateBill.PreviousBills?.Sum(b => b.TotalAmount) ?? 0;
                decimal totalNew = generateBill.NewBills?.Sum(b => b.TotalAmount) ?? 0;
                decimal netTotal = totalPrevious + totalNew;



                var previousBill = generateBill.PreviousBills != null && generateBill.PreviousBills.Any()
                    ? string.Join("", generateBill.PreviousBills.Select(b => $@"
                         <tr>
                             <td>{b.BillTypeName}</td>
                             <td>{b.TotalAmount:F2}</td>           
                         </tr>
                       ")) + $@"<tr style='font-weight:bold; background-color:#f2f2f2;'>
             <td style='text-align:right;'>Total Previous Bills:</td>
             <td style='text-align:right;'>{totalPrevious:F2}</td>
             </tr>"
            : "<tr><td colspan='3'>No previous unpaid bills</td></tr>";



                string newBill = generateBill.NewBills != null && generateBill.NewBills.Any()
              ? string.Join("", generateBill.NewBills.Select(b => $@"
             <tr>
                <td>{b.BillTypeName}</td> 
                <td>{b.TotalAmount:F2}</td>  
             </tr>
            ")) + $@"<tr style='font-weight:bold; background-color:#f2f2f2;'>
             <td style='text-align:right;'>Total New Bills:</td>
             <td style='text-align:right;'>{totalNew:F2}</td>
             </tr>"
                : "<tr><td colspan='3'>No new bills generated</td></tr>";



                string totals = $@" <tr style='font-weight:bold; background-color:#f2f2f2;'>
                 <td colspan='1' style='text-align:left; font-size:16px; padding:8px;'>Total Bill Amount:</td>
                 <td colspan='1' style='text-align:right; font-size:16px; padding:8px;'>{netTotal:F2}</td>
             </tr>";





                string billingMonths = string.Join(", ", generateBill.NewBills.Where(b => b.BillingMonth > 0).Select(b => b.BillingMonth).Distinct());
                string billingYears = string.Join(", ", generateBill.NewBills.Where(b => b.BillingYear > 0).Select(b => b.BillingYear).Distinct());



                string html = htmlTemplate
                //.Replace("{Id}", generateBill.PreviousBills.Bill.ToString())
                //.Replace("{BillType}", model.BillTypeName)
                .Replace("{UserFullName}", userName)
                .Replace("{UserEmail}", Email)
                .Replace("{UserPhone}", Contact)
                .Replace("{org}", organization)
                .Replace("{BillingMonth}", billingMonths)
                .Replace("{BillingYear}", billingYears)
                .Replace("{PreviousBill}", previousBill)
                .Replace("{NewBill}", newBill)
                .Replace("{Totals}", totals)
                .Replace("{GenerationDate}", DateTime.Now.ToString("dd.MM.yyyy HH:mm"));



                //.Replace("{CompanyLogo}", "/images/company-logo.png");



                SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();

                converter.Options.PdfPageSize = SelectPdf.PdfPageSize.A4;
                converter.Options.PdfPageOrientation = SelectPdf.PdfPageOrientation.Portrait;
                converter.Options.MarginLeft = 10;
                converter.Options.MarginRight = 10;
                converter.Options.MarginTop = 10;
                converter.Options.MarginBottom = 10;

                SelectPdf.PdfDocument doc = converter.ConvertHtmlString(html);
                // Define output directory and ensure it exists
                string billsDir = Path.Combine(_webHostEnvironment.WebRootPath, "Bills");
                if (!Directory.Exists(billsDir))
                {
                    Directory.CreateDirectory(billsDir);
                }

                // Define file name and full path
                string fileName = $"Bill_{userName}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                string filePath = Path.Combine(billsDir, fileName);

                // Save PDF to disk
                doc.Save(filePath);

                // Save PDF to memory stream (to return bytes if needed)
                using (MemoryStream ms = new MemoryStream())
                {
                    doc.Save(ms);
                    doc.Close();
                    return ms.ToArray();
                }
            }
            catch(Exception)
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }

        }
    


    }
}
