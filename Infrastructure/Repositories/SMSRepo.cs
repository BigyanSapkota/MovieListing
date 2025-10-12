using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interface.Repository;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
     public class SMSRepo : ISMSRepo
    {
        private readonly ApplicationDbContext _context;
        public SMSRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddSMS(SMS data)
        {
            data.Status = "Sent";
            _context.SMS.Add(data);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteSMS(string MessageSid)
        {
         
            var sms = await _context.SMS.Where(x => x.MessageSid == MessageSid && x.IsDeleted == false ).FirstOrDefaultAsync();
            if (sms == null)
            {
                return false; 
            }
            sms.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<SMS>> GetAllSMS()
        {
            return await _context.SMS.ToListAsync();
        }

        public async Task<List<SMS>> GetSMSByPhoneNumber(string phoneNumber)
        {
            return await _context.SMS.Where(x => x.ToPhoneNumber == phoneNumber).ToListAsync();
        }
    }
}
