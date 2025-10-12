using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using DnsClient;
using DnsClient.Protocol;
using MailKit.Security;
using MailKit.Net.Smtp;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Shared.Helper
{
     public class ValidateEmail
    {
        public async Task<bool> IsRealEmailAsync(string email)
        {
            if (!MailAddress.TryCreate(email, out _))
                return false;

            // 2) Domain and MX check
            var domain = email.Split('@')[1];
            var lookup = new LookupClient();
            var mxRecords = await lookup.QueryAsync(domain, QueryType.MX);
            var mxHost = mxRecords.Answers
                                  .OfType<MxRecord>()
                                  .OrderBy(r => r.Preference)
                                  .Select(r => r.Exchange.Value.TrimEnd('.'))
                                  .FirstOrDefault();

            if (mxHost == null)
                return false; // no MX record → domain cannot receive emails

            // 3) SMTP handshake (connect & verify recipient)
            try
            {
                using var client = new SmtpClient { Timeout = 10 * 1000 };
                await client.ConnectAsync(mxHost, 25, SecureSocketOptions.StartTlsWhenAvailable);

                // Use MailKit's VerifyAsync to check if email exists
                var mailbox = await client.VerifyAsync(email);

                await client.DisconnectAsync(true);
                return mailbox != null; 
            }
            catch
            {
                return false; 
            }
        }
    

   }
}
