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
     public class OrganizationRepo : IOrganizationRepo
    {
        private readonly ApplicationDbContext _context;

        public OrganizationRepo(ApplicationDbContext context)
        {
            _context = context;
        }


        public Task<Organization> AddAsync(Organization org)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Organization>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Organization> GetByIdAsync(Guid id)
        {
          return await _context.Organizations.Where(o => o.Id == id).FirstOrDefaultAsync();
        }

        public async Task ShouldApproveBy(int approveBy)
        {
            //await _context.Organizations.Add();
            await _context.SaveChangesAsync();
        }

        public async Task<Organization> UpdateAsync(Organization org)
        {
            _context.Organizations.Update(org);
            await _context.SaveChangesAsync();
            return org;
        }


        public async Task UpdateShouldApproveByAsync(Guid orgId, int approveBy)
        {
            var org = await _context.Organizations.Where(o => o.Id == orgId).FirstOrDefaultAsync();
            if (org == null)
                throw new Exception("Organization not found");

            org.shouldApprovedBy = approveBy;
            await _context.SaveChangesAsync();
        }

    }
}
