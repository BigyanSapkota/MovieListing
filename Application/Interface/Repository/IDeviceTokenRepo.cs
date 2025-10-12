using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interface.Repository
{
     public interface IDeviceTokenRepo
    {
        Task<DeviceToken?> GetByTokenAsync(string token);
        Task<List<DeviceToken>> GetByUserIdAsync(string userId);
        Task<List<DeviceToken>> GetAllToken();
        Task AddAsync(DeviceToken token);
        Task AddNotificationAsync(Notification notification);
        Task Save();
    }
}
