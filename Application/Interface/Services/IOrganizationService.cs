using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Domain.Entities;

namespace Application.Interface.Services
{
     public interface IOrganizationService
    {
        Task<Organization> CreateAsync(OrganizationVM vm);
        Task<Organization> GetAsync(Guid id);
        Task<IEnumerable<Organization>> GetAllAsync();
        Task<Organization> UpdateAsync(Guid id, OrganizationVM vm);
        Task DeleteAsync(Guid id);
        Task ShouldApproveBy(int approveBy);
        Task UpdateShouldApproveByAsync( int approveBy);
    }
}
