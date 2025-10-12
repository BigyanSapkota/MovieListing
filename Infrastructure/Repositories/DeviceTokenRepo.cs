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
     public class DeviceTokenRepo : IDeviceTokenRepo
    {
        private readonly ApplicationDbContext _context;

        public DeviceTokenRepo(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task AddAsync(DeviceToken token)
        {
            _context.DeviceToken.Add(token);
            await _context.SaveChangesAsync();
        }

        public async Task<DeviceToken?> GetByTokenAsync(string token)
        {
            var result = await _context.DeviceToken.Where(t => t.Token == token.Trim()).FirstOrDefaultAsync();
            return result;

        }

        public Task<List<DeviceToken>> GetByUserIdAsync(string userId)
        {
            return _context.DeviceToken.Where(t => t.UserId == userId).ToListAsync();
        }

        public async Task AddNotificationAsync(Notification notification)
        {
            _context.Notification.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<DeviceToken>> GetAllToken()
        {
           return await _context.DeviceToken.ToListAsync();
        }
    }
}
