using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interface.Services
{
     public interface IPushNotificationService
    {
        Task RegisterTokenAsync(DeviceToken token);
        Task SendNotificationAsync(string token,string title, string body);

        Task SendNotificationsToMultipleUsersAsync(List<string> tokens, string title, string body);
    }
}
