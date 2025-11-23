using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;

namespace Application.Interface.Services
{
     public interface IUserDapperService
    {
        Task<IEnumerable<GetUserDto>> GetAllAsync();
        Task<GetUserDto> GetByIdAsync(string id);
        Task<bool> CreateAsync(RegisterDto dto);
        Task<bool> UpdateAsync(RegisterDto dto);
        Task<bool> DeleteAsync(string id);
    }
}
