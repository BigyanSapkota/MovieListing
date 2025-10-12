using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Application.Interface.Services;
using MailKit.Security;
using MailKit.Net.Smtp;
using MimeKit;
using Org.BouncyCastle.Tls;
using Shared.Helper;
using Microsoft.Extensions.Configuration;

namespace Application.Service
{
    public class EmailService : IEmailService
    {
        private readonly EmailHelper _emailHelper;
        private readonly IConfiguration _configuration;
        public EmailService(EmailHelper emailHelper, IConfiguration configuration)
        {
            _emailHelper = emailHelper;
            _configuration = configuration;
        }


        public void SendVerificationSuccessEmail(string senderName, string senderEmail, string toName, string toEmail, string subject, string body)
        {
             SendEmail(senderName, senderEmail, toName, toEmail, subject, body);
        }


        public void SendVerificationFailedEmail(string senderName, string senderEmail, string toName, string toEmail, string subject, string body)
        {
            SendEmail(senderName, senderEmail, toName, toEmail, subject, body);
        }








        public void SendEmail(string senderName, string senderEmail, string toName, string toEmail, string subject, string body)
        {

            // Create a new MimeMessage object to represent the email

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(senderName, senderEmail));
            message.To.Add(new MailboxAddress(toName, toEmail));
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
