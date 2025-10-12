using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Application.Interface.Repository;
using Application.Interface.Services;
using Domain.Entities;
using FirebaseAdmin.Messaging;
using Google.Apis.Drive.v3.Data;
using Lib.Net.Http.WebPush;
using Lib.Net.Http.WebPush.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Message = FirebaseAdmin.Messaging.Message;


namespace Application.Service
{
    public class PushNotificationService : IPushNotificationService
    {
        private readonly IDeviceTokenRepo _deviceTokenRepo;
        private readonly FirebaseService _firebaseService;
        //private readonly PushServiceClient _pushClient;

        public PushNotificationService(IDeviceTokenRepo deviceTokenRepo, FirebaseService firebaseService)
        {
            _deviceTokenRepo = deviceTokenRepo;
            _firebaseService = firebaseService;

        }


        public async Task RegisterTokenAsync(DeviceToken token)
        {
            if (string.IsNullOrEmpty(token.Token))
                throw new ArgumentException("Token is required");

            var existing = await _deviceTokenRepo.GetByTokenAsync(token.Token);
            if (existing == null)
                await _deviceTokenRepo.AddAsync(token);
        }




        public async Task SendNotificationAsync(string token, string title, string body)
        {
            var user = await _deviceTokenRepo.GetByTokenAsync(token);
            var notification = new Domain.Entities.Notification
            {
                UserId = user?.UserId,
                Token = token,
                Title = title,
                Body = body,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };
            await _deviceTokenRepo.AddNotificationAsync(notification);


            var message = new Message()
            {
                Token = token,
                Notification = new FirebaseAdmin.Messaging.Notification
                {
                    Title = title,
                    Body = body
                }
            };

            try
            {
                var data = await _firebaseService.SendNotificationAsync(message);

                
                notification.Status = "Sent";
                notification.CreatedAt = DateTime.Now;
                await _deviceTokenRepo.Save();
            }
            catch (FirebaseMessagingException ex)
            {
                notification.Status = "Failed";
                await _deviceTokenRepo.Save();

                Console.WriteLine($"Firebase Messaging Error: {ex.Message}");
                throw;
            }

        }






        public async Task SendNotificationsToMultipleUsersAsync(List<string> tokens, string title, string body)
        {
            var notifications = new List<Domain.Entities.Notification>();

            foreach (var token in tokens)
            {
                var user = await _deviceTokenRepo.GetByTokenAsync(token);
                var notification = new Domain.Entities.Notification
                {
                    UserId = user?.UserId,
                    Token = token,
                    Title = title,
                    Body = body,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow
                };
                await _deviceTokenRepo.AddNotificationAsync(notification);
                notifications.Add(notification);
            }

            var message = new MulticastMessage
            {
                Tokens = tokens,
                Notification = new FirebaseAdmin.Messaging.Notification
                {
                    Title = title,
                    Body = body
                }
            };

            try
            {
                var response = await _firebaseService.SendMulticastAsync(message); 

                for (int i = 0; i < response.Responses.Count; i++)
                {
                    if (response.Responses[i].IsSuccess)
                        notifications[i].Status = "Sent";
                    else
                        notifications[i].Status = "Failed";
                }

                await _deviceTokenRepo.Save();
            }
            catch (FirebaseMessagingException ex)
            {
                Console.WriteLine($"Firebase Messaging Error: {ex.Message}");
                notifications.ForEach(n => n.Status = "Failed");
                await _deviceTokenRepo.Save();
                throw;
            }
        }







    }
}










































// public class PushNotificationService
//{
//    private readonly IConfiguration _configuration;

//    public PushNotificationService(IConfiguration configuration)
//    {
//        _configuration = configuration;
//    }


//    public async Task SendNotificationAsync (WebPushRequest request,string message)
//    {
//        var subscription = new PushSubscription
//        {
//            Endpoint = request.Endpoint,
//            Keys = new Dictionary<string, string>
//            {
//                 { "p256dh", request.P256dh },
//                 { "auth", request.Auth }
//            }
//        };

//        var client = new PushServiceClient
//        {
//            DefaultAuthentication = new VapidAuthentication
//            (
//                _configuration["VAPID:publicKey"],
//                _configuration["VAPID:privateKey"]
//            )
//            {
//                Subject = _configuration["VAPID:subject"]
//            }
//        };

//        var notification = new PushMessage(message);
//        await client.RequestPushMessageDeliveryAsync(subscription, notification);
//    }


//}

