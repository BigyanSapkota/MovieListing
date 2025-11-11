using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Application.Interface;
using Application.Interface.Services;
using Application.Service;
using Domain.Entities;
using Infrastructure.Migrations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Infrastructure.Service
{
    public class PdfGenerateService : IPdfGenerateService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        public PdfGenerateService(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor contextAccessor, UserManager<User> userManager, IUnitOfWork unitOfWork)
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
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
                var user = await _userManager.Users
                    .Include(u => u.Organization)
                    .FirstOrDefaultAsync(x => x.Id == userId);

                if (user == null)
                    throw new ArgumentException("User not found!");

                var userName = user.UserName;
                var organization = user.Organization?.Name;
                var email = user.Email;
                var contact = user.PhoneNumber;

                decimal totalPrevious = generateBill.PreviousBills?.Sum(b => b.TotalAmount) ?? 0;
                decimal totalNew = generateBill.NewBills?.Sum(b => b.TotalAmount) ?? 0;
                decimal netTotal = totalPrevious + totalNew;

                string billingMonths = string.Join(", ", generateBill.NewBills.Where(b => b.BillingMonth > 0).Select(b => b.BillingMonth).Distinct());
                string billingYears = string.Join(", ", generateBill.NewBills.Where(b => b.BillingYear > 0).Select(b => b.BillingYear).Distinct());

                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(30);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(12));

                        page.Header()
                            .Text($"Apartment Bill - {organization}")
                            .SemiBold().FontSize(18).FontColor(Colors.Blue.Darken1);

                        page.Content().PaddingVertical(10).Column(col =>
                        {
                           
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                });

                                table.Cell().Element(CellStyle).Text($"Name: {userName}");
                                table.Cell().Element(CellStyle).Text($"Email: {email}");

                                table.Cell().Element(CellStyle).Text($"Contact: {contact}");
                                table.Cell().Element(CellStyle).Text($"Organization: {organization}");
                            });

                            col.Item().PaddingVertical(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                           
                            col.Item().Text($"Billing Month(s): {billingMonths}");
                            col.Item().Text($"Billing Year(s): {billingYears}");

                            col.Item().PaddingVertical(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                          
                            col.Item().Text("Previous Bills").Bold().FontSize(14);
                            if (generateBill.PreviousBills != null && generateBill.PreviousBills.Any())
                            {
                                col.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn(1);
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Element(HeaderCellStyle).Text("Bill Type");
                                        header.Cell().Element(HeaderCellStyle).Text("Amount (Rs)");
                                    });

                                    foreach (var b in generateBill.PreviousBills)
                                    {
                                        table.Cell().Element(CellStyle).Text(b.BillTypeName);
                                        table.Cell().Element(CellStyle).Text($"{b.TotalAmount:F2}");
                                    }

                                    table.Cell().ColumnSpan(2).AlignRight().PaddingTop(5).Text($"Total Previous Bills: {totalPrevious:F2}").Bold();
                                });
                            }
                            else
                            {
                                col.Item().Text("No previous unpaid bills").Italic();
                            }

                           
                            col.Item().PaddingTop(15).Text("New Bills").Bold().FontSize(14);
                            if (generateBill.NewBills != null && generateBill.NewBills.Any())
                            {
                                col.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn(1);
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Element(HeaderCellStyle).Text("Bill Type");
                                        header.Cell().Element(HeaderCellStyle).Text("Amount (Rs)");
                                    });

                                    foreach (var b in generateBill.NewBills)
                                    {
                                        table.Cell().Element(CellStyle).Text(b.BillTypeName);
                                        table.Cell().Element(CellStyle).Text($"{b.TotalAmount:F2}");
                                    }

                                    table.Cell().ColumnSpan(2).AlignRight().PaddingTop(5).Text($"Total New Bills: {totalNew:F2}").Bold();
                                });
                            }
                            else
                            {
                                col.Item().Text("No new bills generated").Italic();
                            }

                            
                            col.Item().PaddingTop(20).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(1);
                                });

                                table.Cell().Element(CellStyle).Text("Total Bill Amount:").Bold();
                                table.Cell().Element(CellStyle).AlignRight().Text($"{netTotal:F2}").Bold();
                            });

                            col.Item().PaddingTop(15).AlignRight().Text($"Generated on: {DateTime.Now:dd.MM.yyyy HH:mm}").FontSize(10).Italic();
                        });

                        page.Footer().AlignCenter().Text(x =>
                        {
                            x.Span("This is a system-generated bill.").FontSize(10).Italic();
                        });
                    });
                });

                // Generate and save the PDF
                string billsDir = Path.Combine(_webHostEnvironment.WebRootPath, "Bills");
                if (!Directory.Exists(billsDir))
                    Directory.CreateDirectory(billsDir);

                string fileName = $"Bill_{userName}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                string filePath = Path.Combine(billsDir, fileName);
                await File.WriteAllBytesAsync(filePath, document.GeneratePdf());

                //await File.WriteAllBytesAsync(filePath, pdfBytes);

                return document.GeneratePdf();
            }
            catch (Exception)
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }
        }


        private IContainer CellStyle(IContainer container)
        {
            return container
                .PaddingVertical(2)
                .PaddingHorizontal(4)
                .BorderBottom(1)
                .BorderColor(Colors.Grey.Lighten2);
        }

        private IContainer HeaderCellStyle(IContainer container)
        {
            return container
                .Padding(4)
                .Background(Colors.Grey.Lighten3)
                .BorderBottom(1)
                .BorderColor(Colors.Grey.Lighten1);
        }








        public async Task<byte[]> GenerateBillAsyn(BillSummaryDto generateBill, string userId)
        {
            var user = await _userManager.Users
                   .Include(u => u.Organization)
                   .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new ArgumentException("User not found.");

            var userName = user.UserName;
            var organization = user.Organization?.Name ?? "N/A";
            var email = user.Email ?? "-";
            var phone = user.PhoneNumber ?? "-";


            decimal totalPrev = generateBill.PreviousBills?.Sum(b => b.TotalAmount) ?? 0;
            decimal totalNew = generateBill.NewBills?.Sum(b => b.TotalAmount) ?? 0;
            decimal netTotal = totalPrev + totalNew;

            // Generate PDF using QuestPDF
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(TextStyle.Default.FontSize(12));

                    // ---------- HEADER ----------
                    page.Header().Background(Colors.Blue.Medium)
                        .PaddingVertical(10)
                        .AlignCenter()
                        .Text("Apartment Bill")
                        .FontSize(16)
                        .Bold()
                        .FontColor(Colors.White);

                    // ---------- CONTENT ----------
                    page.Content().Element(content =>
                    {
                        content.Column(col =>
                        {
                            // User Details
                            col.Item().Padding(15).Background(Colors.Grey.Lighten4)
                                .Border(1).BorderColor(Colors.Grey.Lighten2)
                                .CornerRadius(8)
                                .Column(info =>
                                {
                                    info.Item().Text($" Name: {userName}").Bold();
                                    info.Item().Text($" Email: {email}");
                                    info.Item().Text($" Phone: {phone}");
                                    info.Item().Text($" Organization: {organization}");
                                    info.Item().Text($" Billing Month(s): {string.Join(", ", generateBill.NewBills.Select(b => b.BillingMonth).Distinct())}");
                                    info.Item().Text($" Billing Year(s): {string.Join(", ", generateBill.NewBills.Select(b => b.BillingYear).Distinct())}");
                                });

                            // ---------- PREVIOUS BILLS ----------
                            col.Item().PaddingTop(15)
                                .Text("Previous Bills")
                                .Bold()
                                .FontSize(14)
                                .FontColor(Colors.Blue.Darken2) 
                                .Underline();

                            if (generateBill.PreviousBills != null && generateBill.PreviousBills.Any())
                            {
                                col.Item().PaddingTop(5).Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn(1);
                                    });

                                    // Table Header
                                    table.Header(header =>
                                    {
                                        header.Cell().Element(TableHeaderStyle).Text("Bill Type");
                                        header.Cell().Element(TableHeaderStyle).AlignRight().Text("Amount (Rs)");
                                    });

                                    int index = 0;
                                    foreach (var b in generateBill.PreviousBills)
                                    {
                                        bool isAlt = index++ % 2 == 0;
                                        table.Cell().Element(e => TableCellStyle(e, isAlt)).Text(b.BillTypeName);
                                        table.Cell().Element(e => TableCellStyle(e, isAlt)).AlignRight().Text($"{b.TotalAmount:F2}");
                                    }

                                    // Subtotal
                                    table.Cell().ColumnSpan(2).PaddingTop(8)
                                        .AlignRight().Text($"Total Previous Bills: Rs. {totalPrev:F2}")
                                        .Bold().FontColor(Colors.Black);
                                });
                            }
                            else
                            {
                                col.Item().Text("No previous bills").Italic().FontColor(Colors.Grey.Darken1);
                            }

                            // ---------- NEW BILLS ----------
                            col.Item().PaddingTop(15)
                                .Text("New Bills")
                                .Bold()
                                .FontSize(14)
                                .FontColor(Colors.Blue.Darken2)
                                .Underline();

                            if (generateBill.NewBills != null && generateBill.NewBills.Any())
                            {
                                col.Item().PaddingTop(5).Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn(1);
                                    });

                                    // Table Header
                                    table.Header(header =>
                                    {
                                        header.Cell().Element(TableHeaderStyle).Text("Bill Type");
                                        header.Cell().Element(TableHeaderStyle).AlignRight().Text("Amount (Rs)");
                                    });

                                    int index = 0;
                                    foreach (var b in generateBill.NewBills)
                                    {
                                        bool isAlt = index++ % 2 == 0;
                                        table.Cell().Element(e => TableCellStyle(e, isAlt)).Text(b.BillTypeName);
                                        table.Cell().Element(e => TableCellStyle(e, isAlt)).AlignRight().Text($"{b.TotalAmount:F2}");
                                    }

                                    // Subtotal
                                    table.Cell().ColumnSpan(2).PaddingTop(8)
                                        .AlignRight().Text($"Total New Bills: Rs. {totalNew:F2}")
                                        .Bold().FontColor(Colors.Black);
                                });
                            }
                            else
                            {
                                col.Item().Text("No new bills").Italic().FontColor(Colors.Grey.Darken1);
                            }

                            // ---------- GRAND TOTAL ----------
                            col.Item().PaddingTop(25)
                                .AlignRight()
                                .Element(totalBox =>
                                {
                                    totalBox.Background(Colors.Green.Lighten5)
                                        .Border(1)
                                        .BorderColor(Colors.Green.Darken2)
                                        .CornerRadius(6)
                                        .Padding(12)
                                        .Text($"Net Total Amount: Rs. {netTotal:F2}")
                                        .FontSize(14)
                                        .Bold()
                                        .FontColor(Colors.Green.Darken2);
                                });
                        });
                    });

                    // ---------- FOOTER ----------
                    page.Footer()
                        .AlignCenter()
                        .Text($"Generated on {DateTime.Now:yyyy-MM-dd HH:mm}")
                        .FontSize(10)
                        .FontColor(Colors.Grey.Darken1);
                });
            });

            // ---------- STYLE HELPERS ----------
            IContainer TableHeaderStyle(IContainer container)
            {
                return container.DefaultTextStyle(TextStyle.Default.SemiBold().FontColor(Colors.White))
                    .Background(Colors.Blue.Darken2)
                    .PaddingVertical(6)
                    .PaddingHorizontal(3)
                    .BorderBottom(1)
                    .BorderColor(Colors.Blue.Medium);
            }

           
            // Generate and save the PDF
            string billsDir = Path.Combine(_webHostEnvironment.WebRootPath, "Bills");
            if (!Directory.Exists(billsDir))
                Directory.CreateDirectory(billsDir);

            string fileName = $"Bill_{userName}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            string filePath = Path.Combine(billsDir, fileName);
            await File.WriteAllBytesAsync(filePath, document.GeneratePdf());

            return document.GeneratePdf();


        }


        private IContainer TableCellStyle(IContainer container, bool alternate)
        {
            return container
                .Background(alternate ? Colors.Grey.Lighten4 : Colors.White)
                .PaddingVertical(5)
                .PaddingHorizontal(3)
                .BorderBottom(0.5f)
                .BorderColor(Colors.Grey.Lighten2);
        }











        public async Task<byte[]> GenerateNewBillAsync(BillSummaryDto generateBill, string userId)
        {
            try
            {
                string browserPath = GetBrowserPath();

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

                string billingMonths = string.Join(", ", generateBill.NewBills.Where(b => b.BillingMonth > 0).Select(b => b.BillingMonth).Distinct());
                string billingYears = string.Join(", ", generateBill.NewBills.Where(b => b.BillingYear > 0).Select(b => b.BillingYear).Distinct());


                var previousBill = generateBill.PreviousBills != null && generateBill.PreviousBills.Any()
                   ? string.Join("", generateBill.PreviousBills.Select(b => $@"
                         <tr>
                             <td>{b.BillTypeName}</td>
                             <td>{b.TotalAmount:F2}</td>           
                         </tr>
                       ")) + $@"<tr style='font-weight:bold; background-color:#f2f2f2;'>
             <td style='font-weight:bold'>Total Previous Amount:</td>
             <td style='font-weight:bold'>{totalPrevious:F2}</td>
             </tr>"
           : "<tr><td colspan='3'>No previous unpaid bills</td></tr>";



                string newBill = generateBill.NewBills != null && generateBill.NewBills.Any()
              ? string.Join("", generateBill.NewBills.Select(b => $@"
             <tr>
                <td>{b.BillTypeName}</td> 
                <td>{b.TotalAmount:F2}</td>  
             </tr>
            ")) + $@"<tr>
             <td style='font-weight:bold; font-size:10px '>Total New Amount:</td>
             <td style='font-weight:bold; font-size:10px'>{totalNew:F2}</td>
             </tr>"
                : "<tr><td colspan='3'>No new bills generated</td></tr>";



                string totals = $@" <tr style='font-weight:bold; background-color:#f2f2f2;'>
                 <td colspan='1' style='text-align:left; font-size:16px; padding:8px;'>Total Bill Amount:</td>
                 <td colspan='1' style='text-align:right; font-size:16px; padding:8px;'>{netTotal:F2}</td>
             </tr>";






                //  Load HTML template
                string templatePath = Path.Combine(_webHostEnvironment.WebRootPath, "Templates", "BillTemplate.html");
                

                if (!File.Exists(templatePath))
                    throw new FileNotFoundException("HTML template not found at " + templatePath);

                var html = await File.ReadAllTextAsync(templatePath, Encoding.UTF8);

          
                html = html
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

              
                
                //  Write temp HTML
                var tempHtmlPath = Path.Combine(Path.GetTempPath(), $"bill_{Guid.NewGuid()}.html");
                await File.WriteAllTextAsync(tempHtmlPath, html, Encoding.UTF8);

                //  Define output PDF path
                //var outputPdfPath = Path.Combine(Path.GetTempPath(), $"bill_{Guid.NewGuid()}.pdf");


                string billsDir = Path.Combine(_webHostEnvironment.WebRootPath, "Bills");
                if (!Directory.Exists(billsDir))
                {
                    Directory.CreateDirectory(billsDir);
                }

                string fileName = $"Bill_{organization}_{userName}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                string filePath = Path.Combine(billsDir, fileName);

                // Builds command-line arguments for launching Chrome/Edge in headless mode
                var args = $"--headless --disable-gpu --print-to-pdf-no-header --print-to-pdf=\"{filePath}\" \"file:///{tempHtmlPath.Replace('\\', '/')}\"";
                //Configure ProcessStartInfo. Prepares how the process (Chrome) will start.
                var psi = new ProcessStartInfo
                {
                    FileName = browserPath,
                    Arguments = args,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                // Start Chrome/Edge Process and Generate Pdf
                using (var process = new Process { StartInfo = psi })
                {
                    process.Start();
                    string stderr = await process.StandardError.ReadToEndAsync();
                    string stdout = await process.StandardOutput.ReadToEndAsync();
                    process.WaitForExit(20000); 

                    if (!File.Exists(filePath))
                        throw new Exception($"Failed to create PDF. Browser output: {stderr} {stdout}");
                }

                //  Read PDF as Bytes
                var pdfBytes = await File.ReadAllBytesAsync(filePath);

                //  Remove temp files
                if (File.Exists(tempHtmlPath))
                    File.Delete(tempHtmlPath);


                return pdfBytes;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PdfGenerateService] Error: {ex.Message}");
                return null;
            }
        }



        private string GetBrowserPath()
        {
            var chromePaths = new[]
            {
             @"C:\Program Files\Google\Chrome\Application\chrome.exe",
             @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe"
            };

            var edgePaths = new[]
            {
              @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe",
              @"C:\Program Files\Microsoft\Edge\Application\msedge.exe"
            };

            foreach (var path in chromePaths)
                if (File.Exists(path))
                    return path;

            foreach (var path in edgePaths)
                if (File.Exists(path))
                    return path;

            throw new FileNotFoundException("Neither Chrome nor Edge browser executable found on this system.");
        }





    }

}



