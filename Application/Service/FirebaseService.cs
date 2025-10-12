using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Application.Interface.Services;
using Domain.Entities;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Application.Service
{
    public class FirebaseService
    {
      


        public FirebaseService(IConfiguration configuration)
        {
            if (FirebaseApp.DefaultInstance == null)
            {
                var path = Path.Combine(AppContext.BaseDirectory, "firebase-adminsdk.json");
                var options = new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(path)
                };

                FirebaseApp.Create(options);
                Console.WriteLine("Firebase initialized successfully.");
            }
            else
            {
                Console.WriteLine("Firebase already initialized.");
            }
        }





        public async Task<string> SendNotificationAsync(FirebaseAdmin.Messaging.Message message)
        {

            try
            {
                return await FirebaseMessaging.DefaultInstance.SendAsync(message);
            }
            catch (FirebaseMessagingException ex)
            {
                Console.WriteLine($"Firebase Messaging Error: {ex.Message}");
                throw;

            }

        }



        public async Task<BatchResponse> SendMulticastAsync(MulticastMessage message)
        {
            try
            {
                return await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(message);
            }
            catch (FirebaseMessagingException ex)
            {
                Console.WriteLine($"Firebase Messaging Error: {ex.Message}");
                throw;
            }
        }





        }
}