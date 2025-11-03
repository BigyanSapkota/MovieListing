using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO.Movie;
using Application.Interface.Repository;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Common;


namespace Infrastructure.Repositories
{
     public class DeleteRequestRepo : IDeleteRequestRepo
    {
        private readonly ApplicationDbContext _context;

        public DeleteRequestRepo(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task AddApprovalAsync(DeleteApproval approvalEntity)
        {
             _context.DeleteApproval.Add(approvalEntity);
            await _context.SaveChangesAsync();
        }



        public async Task CreateDeleteRequestAsync(DeleteRequest entity)
        {
            _context.DeleteRequest.Add(entity);
            await _context.SaveChangesAsync();

        }

        public async Task<IEnumerable<DeleteRequest>> GetPendingRequestsAsync()
        {
            return await _context.DeleteRequest.Where(r => r.Status == "Pending").ToListAsync();

        }

        public async Task<DeleteRequest?> GetRequestByIdAsync(Guid id)
        {
            var request = await _context.DeleteRequest.Include(r => r.Approval).FirstOrDefaultAsync(r => r.DeleteRequestId == id);
           return request;


        }

        public async Task<DeleteRequest> GetRequestByMovie(Guid movieId)
        {
            return await _context.DeleteRequest.FirstOrDefaultAsync((x => x.MovieId == movieId));
        }

        public Task<bool> HasAdminApprovedAsync(Guid requestId, string adminId)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateRequestAsync(DeleteRequest request)
        {
            _context.DeleteRequest.Update(request);
            
           await _context.SaveChangesAsync();
            
        }


    }
}
