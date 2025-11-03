using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO.Movie;
using Domain.Entities;

namespace Application.Interface.Services
{
    public interface IDeleteRequestService
    {
        Task<DeleteRequestVM> CreateDeleteRequestAsync(Guid movieId, string adminId);
        Task<DeleteRequestVM> ApproveDeleteRequestAsync(Guid requestId);
        Task<IEnumerable<DeleteRequestVM>> GetPendingRequestsAsync();
        Task<DeleteRequest> GetRequestByMovie(Guid movieId);
    }
}
