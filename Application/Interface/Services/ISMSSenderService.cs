using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interface.Services
{
     public interface ISMSSenderService
    {
        Task<SMS> SendSMSAsync(string phoneNumber, string message);
        Task<List<SMS>> GetAllSMSAsync();
        Task<List<SMS>> GetSMSByPhoneNumberAsync(string phoneNumber);
        Task<bool> DeleteSMSAsync(string MessageSid);

    }
}
