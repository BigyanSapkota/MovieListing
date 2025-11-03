using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO.Movie;
using Domain.Entities;

namespace Application.Interface.Repository
{
     public interface IDeleteRequestRepo
    {
        Task<DeleteRequest?> GetRequestByIdAsync(Guid id);
        Task CreateDeleteRequestAsync(DeleteRequest entity);
        Task AddApprovalAsync(DeleteApproval approvalEntity);
        Task<bool> HasAdminApprovedAsync(Guid requestId, string adminId);
        Task UpdateRequestAsync(DeleteRequest request);
        Task<IEnumerable<DeleteRequest>> GetPendingRequestsAsync();
        Task<DeleteRequest> GetRequestByMovie(Guid movieId);
    }
}
