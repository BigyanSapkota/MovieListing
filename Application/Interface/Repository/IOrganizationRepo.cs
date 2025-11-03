using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interface.Repository
{
     public interface IOrganizationRepo
    {
        Task<Organization> AddAsync(Organization org);
        Task<Organization> GetByIdAsync(Guid id);
        Task<IEnumerable<Organization>> GetAllAsync();
        Task<Organization> UpdateAsync(Organization org);
        Task DeleteAsync(Guid id);
        Task ShouldApproveBy(int approveBy);
        Task UpdateShouldApproveByAsync(Guid orgId, int approveBy);
    }
}
