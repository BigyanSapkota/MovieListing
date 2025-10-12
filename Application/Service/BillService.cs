using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Application.Interface;
using Application.Interface.Repository;
using Application.Interface.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Vonage.SubAccounts.TransferAmount;
using static System.Net.Mime.MediaTypeNames;

namespace Application.Service
{
     public class BillService : IBillService
    {
        private readonly IBillRepo _billRepo;
        private readonly IBillTypeRepo _billTypeRepo;
        private readonly IGenericRepo<Bill, Guid> _genericRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        public BillService(IBillRepo billRepo, IGenericRepo<Bill, Guid> genericRepo,IUnitOfWork unitOfWork, IBillTypeRepo billTypeRepo,UserManager<User> userManager)
        {
            _billRepo = billRepo;
            _genericRepo = genericRepo;
            _unitOfWork = unitOfWork;
            _billTypeRepo = billTypeRepo;
            _userManager = userManager;
        }



        public async Task<Bill> CreateAsync(CreateBill billDto)
        {
           var bill = new Bill
            {
                //Id = Guid.NewGuid(),
                BillTypeId = billDto.BillTypeId,
                UserId = billDto.UserId,
                TotalAmount = billDto.TotalAmount,
                //PaidAt = billDto.PaidAt,
                //IsPaid = billDto.IsPaid,
                //BillingMonth = billDto.BillingMonth,
                //BillingYear = billDto.BillingYear
            };
            await _genericRepo.AddAsync(bill);
            await _unitOfWork.CommitAsync();
            return bill;
        }

        public Task<Guid> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

      

        public async Task<IEnumerable<BillDTO>> GetAllAsync()
        {
            var bills = await _genericRepo.GetAllAsync();
            return bills.Select(b => new BillDTO
            {
                Id = b.Id,
                BillTypeId = b.BillTypeId,
                UserId = b.UserId,
                TotalAmount = b.TotalAmount,
                PaidAt = b.PaidAt,
                IsPaid = b.IsPaid,
                //BillingMonth = b.BillingMonth,
                //BillingYear = b.BillingYear
            });
        }




        public async Task<BillDTO> GetByIdAsync(Guid id)
        {

            var bill = await _billRepo.GetBillByIdAsync(id);
            if (bill == null) return null;

            return new BillDTO
            {
                Id = bill.Id,
                BillTypeId = bill.BillTypeId,
                UserId = bill.UserId,
                TotalAmount = bill.TotalAmount,
                PaidAt = bill.PaidAt,
                IsPaid = bill.IsPaid,
                //BillingMonth = bill.BillingMonth,
                //BillingYear = bill.BillingYear
            };
        }

        public async Task<IEnumerable<BillDTO>> GetByUserIdAsync(string userId)
        {
            var bill = await _billRepo.GetAllUserBillsAsync(userId);
            if (bill == null || !bill.Any()) return null;
            return bill.Select(b => new BillDTO
            {
                Id = b.Id,
                BillTypeId = b.BillTypeId,
                UserId = b.UserId,
                TotalAmount = b.TotalAmount,
                PaidAt = b.PaidAt,
                IsPaid = b.IsPaid,
                //BillingMonth = b.BillingMonth,
                //BillingYear = b.BillingYear
            });

        }

        public async Task<Bill> UpdateAsync(BillDTO billDto)
        {
           var bill = new Bill
            {
                Id = billDto.Id,
                BillTypeId = billDto.BillTypeId,
                UserId = billDto.UserId,
                TotalAmount = billDto.TotalAmount,
                PaidAt = billDto.PaidAt,
                IsPaid = billDto.IsPaid,
                //BillingMonth = billDto.BillingMonth,
                //BillingYear = billDto.BillingYear
            };
            await _genericRepo.UpdateAsync(bill);
            await _unitOfWork.CommitAsync();
            return bill;
        }





        public async Task<List<CreateBill>> GenerateNextBillsAsync(string userId)
        {
            var billTypes = await _billTypeRepo.GetAllBillType();
            var generatedBills = new List<CreateBill>();
            foreach (var billType in billTypes)
            {
                var latestBill = await _billRepo.GetLatestBillAsync(userId, billType.Id);
                Bill newBill = null;
                if (latestBill == null)
                {
                    newBill = new Bill
                    {
                        BillTypeId = billType.Id,
                        UserId = userId,
                        TotalAmount = billType.DefaultAmount,
                        //BillingMonth = month,
                        //BillingYear = year,
                        //NextGenerateDate = new DateTime(year, month, 1).AddMonths(billType.interval+1)
                    };

                    await _genericRepo.AddAsync(newBill);
                    await _unitOfWork.CommitAsync();
                    
                    //generatedBills.Add(newBill);
                }
                else
                {
                    string oldDate = latestBill.CreatedAt.ToString();
                    DateTime old_date = DateTime.Parse(oldDate);
                    int oldYear = old_date.Year;
                    int oldMonth = old_date.Month;

                    string createdDate = DateTime.Now.ToString();
                    DateTime date = DateTime.Parse(createdDate);

                    int Year = date.Year;
                    int Month = date.Month;

                    int months = ((Year - oldYear) * 12) + (Month - oldMonth);



                    if (billType.IsFixedAmount)
                    {
                        if (months >= billType.interval)
                        {
                            decimal Amount = 0;
                            Amount = billType.DefaultAmount;
                            newBill = new Bill
                            {
                                BillTypeId = billType.Id,
                                UserId = userId,
                                TotalAmount = Amount,
                                //BillingMonth = month,
                                //BillingYear = year,
                                //NextGenerateDate = new DateTime(latestBill.BillingYear, latestBill.BillingMonth, 1)
                                //       .AddMonths(billType.interval+1)
                            };
                            await _genericRepo.AddAsync(newBill);
                            await _unitOfWork.CommitAsync();
                            //generatedBills.Add(newBill);
                        }
                    }
                    else
                    {
                        if (months == 0)
                        {
                            decimal Amount = 0;
                            Amount = latestBill.TotalAmount;
                            newBill = new Bill
                            {
                                BillTypeId = billType.Id,
                                UserId = userId,
                                TotalAmount = Amount,
                                //BillingMonth = month,
                                //BillingYear = year,
                                //NextGenerateDate = new DateTime(latestBill.BillingYear, latestBill.BillingMonth, 1)
                                //       .AddMonths(billType.interval+1)
                            };

                            //generatedBills.Add(newBill);
                        }
                    }
                }

                if (newBill != null)
                {
                    DateTime previousMonthDate = DateTime.Now.AddMonths(-1);
                    int PYear = previousMonthDate.Year;
                    int PMonth = previousMonthDate.Month;


                    string createdDate = DateTime.Now.ToString();
                    DateTime date = DateTime.Parse(createdDate);

                    int Year = date.Year;
                    int Month = date.Month;
                    var dto = new CreateBill
                    {
                        //Id = newBill.Id,
                        BillTypeId = newBill.BillTypeId,
                        //BillTypeName = billType.Name,
                        UserId = newBill.UserId,
                        TotalAmount = newBill.TotalAmount,
                        //IsPaid = newBill.IsPaid,
                        //PaidAt = newBill.PaidAt,
                        BillingMonth = PMonth,
                        BillingYear = PYear,
                        CreatedAt = newBill.CreatedAt,
                        //NextGenerateDate = newBill.NextGenerateDate
                    };
                    generatedBills.Add(dto);
                }
            }
            return generatedBills;
        }








        //public async Task<List<Bill>> GenerateNextBillsAsync(string userId,int month , int year)
        //
        //    var billTypes = await _billTypeRepo.GetAllBillType();
        //    var generatedBills = new List<Bill>();
        //    foreach (var billType in billTypes)
        //    {
        //        var latestBill =await _billRepo.GetLatestBillAsync( userId, billType.Id);

        //        int months = ((year - latestBill.BillingYear) * 12) + (month - latestBill.BillingMonth);

        //        if(months>=billType.interval)
        //        {

        //            if (billType.interval == 1)
        //            {
        //                var newBill = new Domain.Entities.Bill
        //                {
        //                    BillTypeId = latestBill.BillTypeId,
        //                    UserId = latestBill.UserId,
        //                    TotalAmount = latestBill.TotalAmount,
        //                    BillingMonth = month,
        //                    BillingYear = year,
        //                    NextGenerateDate = new DateTime(latestBill.BillingYear, latestBill.BillingMonth, 1).AddMonths(billType.interval)
        //                };
        //                generatedBills.Add(newBill);

        //            }
        //            else
        //            {
        //                var newBill = new Domain.Entities.Bill
        //                {
        //                    BillTypeId = latestBill.BillTypeId,
        //                    UserId = latestBill.UserId,
        //                    TotalAmount = latestBill.TotalAmount,
        //                    BillingMonth = month,
        //                    BillingYear = year,
        //                    NextGenerateDate = new DateTime(latestBill.BillingYear, latestBill.BillingMonth, 1).AddMonths(billType.interval)
        //                };


        //                var bill = new Bill
        //                {
        //                    //Id = Guid.NewGuid(),
        //                    BillTypeId = latestBill.BillTypeId,
        //                    UserId = latestBill.UserId,
        //                    TotalAmount = latestBill.TotalAmount,
        //                    //PaidAt = billDto.PaidAt,
        //                    //IsPaid = billDto.IsPaid,
        //                    BillingMonth = newBill.BillingMonth,
        //                    BillingYear = newBill.BillingYear,
        //                    NextGenerateDate = newBill.NextGenerateDate?.AddMonths(billType.interval)
        //                };
        //                await _genericRepo.AddAsync(bill);
        //                await _unitOfWork.CommitAsync();
        //                generatedBills.Add(newBill);
        //            }
        //        }
        //    }
        // return await Task.FromResult(generatedBills);

        //}













        //public async Task<List<Bill>> GenerateNextBillsAsync()
        //{
        //    var generatedBills = new List<Bill>();
        //    var billTypes = await  _billTypeRepo.GetAllBillType();
        //    var bill  = await _genericRepo.GetAllAsync();

        //    var userBills = bill.GroupBy(b => new {b.UserId , b.BillTypeId }).ToList();

        //    foreach (var bills in userBills)
        //    {
        //        var latestBill = bills.OrderByDescending(b => b.BillingYear).ThenByDescending(b => b.BillingMonth).FirstOrDefault();
        //        var billType = billTypes.FirstOrDefault(bt => bt.Id == latestBill?.BillTypeId);

        //        var nextDate = new DateTime(latestBill.BillingYear, latestBill.BillingMonth, 1)
        //                       .AddMonths(billType.interval);

        //        //var exists = bill.Any(b =>
        //        //    b.UserId == latestBill.UserId &&
        //        //    b.BillTypeId == latestBill.BillTypeId &&
        //        //    b.BillingMonth == nextDate.Month &&
        //        //    b.BillingYear == nextDate.Year);


        //        if (!exists && nextDate <= DateTime.Now)
        //        {
        //            var newBill = new Bill
        //            {
        //                BillTypeId = latestBill.BillTypeId,
        //                UserId = latestBill.UserId,
        //                TotalAmount = latestBill.TotalAmount,
        //                NextGenerateDate = nextDate,
        //            };

        //            generatedBills.Add(newBill);

        //            await _genericRepo.AddAsync(newBill);


        //        }
        //    }
        //    if (generatedBills.Any())
        //        await _unitOfWork.CommitAsync();

        //    return generatedBills;

        //}



    }
}
