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
     public class GoogleAuthRepo : IGoogleAuthRepo
    {
        private readonly ApplicationDbContext _context;
        public GoogleAuthRepo(ApplicationDbContext context)
        {
            _context = context;
        }



        public async Task SaveRefreshTokenAsync(RefreshToken token)
        {
            _context.RefreshToken.Add(token);
            await _context.SaveChangesAsync();
        }

        public async Task<RefreshToken>GetRefreshTokenAsync(string token)
        {
            return await _context.RefreshToken.Where(r => r.Token == token && !r.IsRevoked).FirstOrDefaultAsync();
        }


        public async Task RevokeRefreshTokenAsync(string token)
        {
            var refresh = await _context.RefreshToken.FirstOrDefaultAsync(r => r.Token == token);
            if (refresh != null)
            {
                refresh.IsRevoked = true;
                await _context.SaveChangesAsync();
            }
        }


    }
}
