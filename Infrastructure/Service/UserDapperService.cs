using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Application.Interface.Repository;
using Application.Interface.Services;
using Domain.Entities;

namespace Infrastructure.Service
{
     public class UserDapperService : IUserDapperService
    {
        private readonly IUserDapperRepo _userDapperRepo;
        public UserDapperService(IUserDapperRepo userDapperRepo)
        {
            _userDapperRepo = userDapperRepo;
        }

        public Task<bool> CreateAsync(RegisterDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<GetUserDto>> GetAllAsync()
        {
            var users = await _userDapperRepo.GetAllUserAsync();
            return users.Select(static u => new GetUserDto
            {
                UserId = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                Role = u.Roles != null && u.Roles.Any() ? string.Join(", ", u.Roles) : null,
                PhoneNumber = u.PhoneNumber
            });
        }

        public async Task<GetUserDto> GetByIdAsync(string id)
        {
            var user = await _userDapperRepo.GetUserByIdAsync(id);
            if (user == null) return null;
            return new GetUserDto
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };

        }

        public Task<bool> UpdateAsync(RegisterDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
