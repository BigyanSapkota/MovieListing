using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interface.Repository;
using Application.Interface.Services;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Application.Service
{
     public class SMSSenderService : ISMSSenderService
    {
        private readonly IConfiguration _configuration;
        private readonly string? AccountSID;
        private readonly string? AuthToken;
        private readonly string? FromNumber;
        private readonly ISMSRepo _smsRepo;

        public SMSSenderService(IConfiguration configuration,ISMSRepo smsRepo)
        {
            _configuration = configuration;
            AccountSID = _configuration["SMSSetting:AccountSID"];
            AuthToken = _configuration["SMSSetting:AuthToken"];
            FromNumber = _configuration["SMSSetting:FromNumber"];
            _smsRepo = smsRepo;
        }

        public async Task<bool> DeleteSMSAsync(string MessageSid)
        {
            return await _smsRepo.DeleteSMS(MessageSid);
        }

        public async Task<List<SMS>> GetAllSMSAsync()
        {
            return await _smsRepo.GetAllSMS();
        }

        public async Task<List<SMS>> GetSMSByPhoneNumberAsync(string phoneNumber)
        {
            return await _smsRepo.GetSMSByPhoneNumber(phoneNumber);
        }





        public async Task<SMS> SendSMSAsync(string phoneNumber, string message)
        {
            try
            {
                if (string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(message))
                {
                    throw new ArgumentException("Phone number and message are required");
                }

                if (!phoneNumber.StartsWith("+"))
                {
                    phoneNumber = "+" + phoneNumber;
                }

                //Initialize Twilio client
                TwilioClient.Init(AccountSID, AuthToken);

                // set recipient phone number and message body
                var messageOptions = new CreateMessageOptions(new PhoneNumber(phoneNumber))
                {
                    From = new PhoneNumber(FromNumber),

                    Body = message
                };

                // Use the Twilio API to send the SMS
                var msg = await MessageResource.CreateAsync(messageOptions);

                var data = new SMS
                {
                    Id = Guid.NewGuid(),
                    FromPhoneNumber = msg.From.ToString(),
                    ToPhoneNumber = msg.To.ToString(),
                    Message = msg.Body,
                    MessageSid = msg.Sid,
                    Status = msg.Status.ToString(),
                    ErrorMessage = msg.ErrorMessage,
                    CreatedAt = msg.DateCreated,
                    IsDeleted = false
                };

                await _smsRepo.AddSMS(data);
                
                return data;

               
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending SMS: {ex.Message}");
                return null;

            }
        }




    }
}
