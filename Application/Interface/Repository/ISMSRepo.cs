using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interface.Repository
{
     public interface ISMSRepo
    {
        Task AddSMS(SMS data);
        Task<List<SMS>> GetAllSMS();
        Task<List<SMS>> GetSMSByPhoneNumber(string phoneNumber);
        Task<bool> DeleteSMS (string MessageSid);

    }
}
