using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Vonage;
using Vonage.Messaging;
using Vonage.Request;

namespace Application.Service
{
     public class VonageSMSService
    {
        private readonly IConfiguration _configuration;
        public VonageSMSService(IConfiguration configuration)
        {
            _configuration = configuration;     
        }



        public async Task<SendSmsResponse> SendSMSAsync(string ToNumber,string message)
        {
            var credentials = Credentials.FromApiKeyAndSecret(
                  _configuration["VonageSMS:ApiKey"],
                  _configuration["VonageSMS:ApiSecret"]
                );
            var vonageClient = new VonageClient(credentials);

            var response = await vonageClient.SmsClient.SendAnSmsAsync(new Vonage.Messaging.SendSmsRequest()
            {
                To = ToNumber,
                From = _configuration["VonageSMS:From"],
                Text = message
            });
            Console.WriteLine(response.Messages[0].To);
            return response;
        }



    }
}
