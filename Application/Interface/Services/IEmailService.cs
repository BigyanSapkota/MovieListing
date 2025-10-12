using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface.Services
{
     public interface IEmailService
    {
        void SendEmail(string senderName, string senderEmail, string toName, string toEmail, string subject, string body);
        void SendVerificationSuccessEmail(string senderName, string senderEmail, string toName, string toEmail, string subject, string body);
        void SendVerificationFailedEmail(string senderName, string senderEmail, string toName, string toEmail,string subject, string body);
    }




}
