using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Shared.Helper
{
     public class EmailHelper
    {

        private readonly IConfiguration _configuration;
        public EmailHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }



        public void SendEmail(string senderName, string senderEmail, string toName, string toEmail, string subject, string body)
        {

            // Create a new MimeMessage object to represent the email

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(senderName, senderEmail));
            message.To.Add(new MailboxAddress(toName,toEmail));
            message.Subject = subject;
            message.Body = new TextPart("plain")
            {
                Text = body
            };

            // Create a new SMTP client to send the email

            using (var client = new SmtpClient())
            {
                // Connect to the SMTP server using the configuration settings
                client.Connect(
                    _configuration["EmailSettings:SmtpServer"],
                    int.Parse(_configuration["EmailSettings:SmtpPort"]),
                    SecureSocketOptions.StartTls
                    );



                // Authenticate with the SMTP server using the configuration settings
                client.Authenticate(_configuration["EmailSettings:Username"], _configuration["EmailSettings:Password"]);

                // Send the email message
                client.Send(message);

                // Disconnect from the SMTP server
                client.Disconnect(true);


            }


        }




    }
}
