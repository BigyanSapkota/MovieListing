using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO.Movie;
using Application.Interface;
using Application.Interface.Repository;
using Application.Interface.Services;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Shared.Helper;


namespace Infrastructure.Service
{
     public class DeleteRequestService : IDeleteRequestService
    {
        private readonly IDeleteRequestRepo _deleteRequestRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IGenericRepo<Movie,Guid> _movieRepo;
        private readonly IMovieRepo _movieRepoo;
        private readonly IMapper _mapper;
        private readonly IOrganizationRepo _organizationRepo;
        public DeleteRequestService(IDeleteRequestRepo deleteRequestRepo,IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor, IGenericRepo<Movie, Guid> movieRepo, IMovieRepo movieRepoo,IMapper mapper,IOrganizationRepo organizationRepo)
        {
            _deleteRequestRepo = deleteRequestRepo;
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
            _movieRepo = movieRepo;
            _movieRepoo = movieRepoo;
            _mapper = mapper;
            _organizationRepo = organizationRepo;
        }

        //public async Task<DeleteRequestVM> ApproveDeleteRequestAsync(Guid requestId)
        //{
        //    var user = _contextAccessor.HttpContext?.User;
        //    string? loggedInUser = UserInfoHelper.GetUserId(user);
        //    if (loggedInUser == null)
        //    {
        //        throw new UnauthorizedAccessException("User not authenticated");
        //    }
        //    var request = await _deleteRequestRepo.GetRequestByIdAsync(requestId);
        //    if (request == null)
        //    {
        //        throw new KeyNotFoundException("Delete request not found");
        //    }

        //    if (request.RequestedByAdminId == loggedInUser)
        //    {
        //        throw new ArgumentException("You cannot approve your own request.");
        //    }

        //    bool alreadyApproved = request.Approval.Any(a => a.ApprovedByAdminId == loggedInUser);
        //    if (alreadyApproved)
        //    {
        //        throw new ArgumentException("You have already approved this request.");
        //    }

        //    var approvalEntity = new DeleteApproval
        //    {
        //        ApprovalId = Guid.NewGuid(),
        //        DeleteRequestId = requestId,
        //        ApprovedByAdminId = loggedInUser,
        //        ApprovedAt = DateTime.Now
        //    };

        //    await _deleteRequestRepo.AddApprovalAsync(approvalEntity);


        //    request.ApprovedCount = request.ApprovedCount + 1;

        //    if (request.ApprovedCount >= 2)
        //    {
        //        request.Status = "Approved";
        //        //await _deleteRequestRepo.UpdateRequestAsync(request);
        //        //var movie = await _movieRepo.GetByIdAsync(request.MovieId);
        //        var movie = await _movieRepoo.GetMovieByIdAsync(request.MovieId);

        //        //await _movieRepo.UpdateAsync(movie.Data);
        //        if (movie.Data != null)
        //        {
        //            await _movieRepoo.DeleteAsync(movie.Data);

        //        }

        //    }
        //    await _deleteRequestRepo.UpdateRequestAsync(request);

        //    var vm = new DeleteRequestVM
        //    {
        //        RequestId = request.DeleteRequestId,
        //        MovieId = request.MovieId,
        //        Status = request.Status,
        //        RequestedByAdminId = request.RequestedByAdminId,
        //        ApprovedCount = request.ApprovedCount,
        //        CreatedAt = request.CreatedAt
        //    };
        //    return vm;

        //}



        //public async Task<DeleteRequestVM> ApproveDeleteRequestAsync(Guid requestId)
        //{

        //    var user = _contextAccessor.HttpContext?.User;
        //    string? loggedInUser = UserInfoHelper.GetUserId(user);
        //    if (loggedInUser == null)
        //        throw new UnauthorizedAccessException("User not authenticated");

        //    var request = await _deleteRequestRepo.GetRequestByIdAsync(requestId);
        //    if (request == null)
        //        throw new KeyNotFoundException("Delete request not found");

        //    if (request.RequestedByAdminId == loggedInUser)
        //        throw new ArgumentException("You cannot approve your own request.");

        //    bool alreadyApproved = request.Approval.Any(a => a.ApprovedByAdminId == loggedInUser);
        //    if (alreadyApproved)
        //        throw new ArgumentException("You have already approved this request.");

        //    var approvalEntity = new DeleteApproval
        //    {
        //        ApprovalId = Guid.NewGuid(),
        //        DeleteRequestId = requestId,
        //        ApprovedByAdminId = loggedInUser,
        //        ApprovedAt = DateTime.Now
        //    };
        //    await _deleteRequestRepo.AddApprovalAsync(approvalEntity);

        //    request.ApprovedCount += 1;

        //    if (request.ApprovedCount >= 2)
        //    {
        //        request.Status = "Approved";

        //        var movieDto = await _movieRepoo.GetMovieByIdAsync(request.MovieId);

        //        if (movieDto.Data != null)
        //        {
        //            var movieEntity = movieDto.Data;
        //            movieEntity.IsActive = false;
        //           await _movieRepoo.Update(movieEntity); 
        //        }
        //    }


        //    await _deleteRequestRepo.UpdateRequestAsync(request);


        //    var vm = new DeleteRequestVM
        //    {
        //        RequestId = request.DeleteRequestId,
        //        MovieId = request.MovieId,
        //        Status = request.Status,
        //        RequestedByAdminId = request.RequestedByAdminId,
        //        ApprovedCount = request.ApprovedCount,
        //        CreatedAt = request.CreatedAt
        //    };

        //    return vm;
        //}



        public async Task<DeleteRequestVM> ApproveDeleteRequestAsync(Guid requestId)
        {

            var user = _contextAccessor.HttpContext?.User;
            string? loggedInUser = UserInfoHelper.GetUserId(user);
            var orgId = UserInfoHelper.GetOrganizationId(user);
            var role = UserInfoHelper.GetUserRole(user);
            if (loggedInUser == null || role != "Admin" )
                throw new UnauthorizedAccessException("User not authenticated");

            var request = await _deleteRequestRepo.GetRequestByIdAsync(requestId);
            if (request == null)
                throw new KeyNotFoundException("Delete request not found");
            if (request.RequestedByAdminId == loggedInUser)
                throw new ArgumentException("You cannot approve your own request.");

            bool alreadyApproved = request.Approval.Any(a => a.ApprovedByAdminId == loggedInUser);
            if (alreadyApproved)
                throw new ArgumentException("You have already approved this request.");
            
            var org = await _organizationRepo.GetByIdAsync(orgId);
            var approvalCount = org.shouldApprovedBy;

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var approvalEntity = new DeleteApproval
                {
                    ApprovalId = Guid.NewGuid(),
                    DeleteRequestId = requestId,
                    ApprovedByAdminId = loggedInUser,
                    ApprovedAt = DateTime.Now
                };
                await _deleteRequestRepo.AddApprovalAsync(approvalEntity);

                request.ApprovedCount += 1;
                if (request.ApprovedCount >= approvalCount)
                {

                    var movie = await _movieRepoo.GetMovieByIdAsync(request.MovieId);
                    if (movie == null || movie.Data == null)
                        throw new KeyNotFoundException("Movie not found");

                    //movie.Data.DeletedBy = request.RequestedByAdminId;
                    //movie.Data.DeletedAt = DateTime.Now;
                    await _movieRepoo.DeleteAsync(movie.Data.Id,request.RequestedByAdminId);
                    //await  _movieRepo.DeleteAsync(movie.Data);
                    await _unitOfWork.CommitAsync();

                    request.Status = "Approved";
                    request.ApprovedCount = request.ApprovedCount;
                }

                await _deleteRequestRepo.UpdateRequestAsync(request);
                var vm = new DeleteRequestVM
                {
                    RequestId = request.DeleteRequestId,
                    MovieId = request.MovieId,
                    Status = request.Status,
                    RequestedByAdminId = request.RequestedByAdminId,
                    ApprovedCount = request.ApprovedCount,
                    CreatedAt = request.CreatedAt
                };

                return vm;
            }
            catch(Exception)
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }
        }





        public async Task<DeleteRequestVM> CreateDeleteRequestAsync(Guid movieId, string adminId)
        {
            var entity = new DeleteRequest
            {
                DeleteRequestId = Guid.NewGuid(),
                MovieId = new Guid(movieId.ToString()),
                RequestedByAdminId = adminId.ToString(),
                Status = "Pending",
                ApprovedCount = 1,
                CreatedAt = DateTime.Now
            };
            await _deleteRequestRepo.CreateDeleteRequestAsync(entity);

            var vm = new DeleteRequestVM
            {
                RequestId = entity.DeleteRequestId,
                MovieId = movieId,
                Status = entity.Status,
                RequestedByAdminId = adminId,
                ApprovedCount = entity.ApprovedCount,
                CreatedAt = entity.CreatedAt
            };
            return vm;

            
        }



        public async Task<IEnumerable<DeleteRequestVM>> GetPendingRequestsAsync()
        {
           var data = await _deleteRequestRepo.GetPendingRequestsAsync();
            var response = data.Select(r => new DeleteRequestVM
                {
                RequestId = r.DeleteRequestId,
                MovieId = r.MovieId,
                Status = r.Status,
                RequestedByAdminId = r.RequestedByAdminId,
                ApprovedCount = r.ApprovedCount,
                CreatedAt = r.CreatedAt
            });
            return response;

        }

        public async Task<DeleteRequest> GetRequestByMovie(Guid movieId)
        {
            return await _deleteRequestRepo.GetRequestByMovie(movieId);
        }
    }
}
