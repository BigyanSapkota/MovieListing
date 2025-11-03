using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Application.Interface.Repository;
using Application.Interface.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Shared.Helper;

namespace Infrastructure.Service
{
     public class OrganizationService : IOrganizationService
    {
        private readonly IOrganizationRepo _repo;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrganizationService(IOrganizationRepo repo,IHttpContextAccessor httpContextAccessor)
        {
            _repo = repo;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<Organization> CreateAsync(OrganizationVM vm)
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

        public Task<Organization> GetAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task ShouldApproveBy(int approveBy)
        {
            var user =  _httpContextAccessor.HttpContext?.User;
            var userId = UserInfoHelper.GetUserId(user);
           
            var orgId = UserInfoHelper.GetOrganizationId(user);
            
            var data = await _repo.GetByIdAsync(orgId);
            data.shouldApprovedBy = approveBy;
            await _repo.UpdateAsync(data);
           
        }


        public async Task UpdateShouldApproveByAsync( int approveBy)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var userId = UserInfoHelper.GetUserId(user);

            var orgId = UserInfoHelper.GetOrganizationId(user);

            var data = await _repo.GetByIdAsync(orgId);
            data.shouldApprovedBy = approveBy;
             await _repo.UpdateShouldApproveByAsync(orgId, approveBy);
            
        }



        public Task<Organization> UpdateAsync(Guid id, OrganizationVM vm)
        {
            throw new NotImplementedException();
        }
    }
}
