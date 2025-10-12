using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.Extensions.Configuration;
using static Google.Apis.Drive.v3.DriveService;


namespace Shared.Helper
{
     public class UploadToDrive
    {
        private readonly IConfiguration _configuration;
        public UploadToDrive(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        private DriveService GetService()
        {
            var tokenResponse = new TokenResponse
            {
                AccessToken = _configuration["GoogleDrive:AccessToken"],
                RefreshToken = _configuration["GoogleDrive:RefreshToken"],
            };

            var applicationName = "MovieAuth";// Use the name of the project in Google Cloud
            var username = "code1today@gmail.com"; // Use your email

            var apiCodeFlow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = _configuration["GoogleDrive:ClientId"],
                    ClientSecret = _configuration["GoogleDrive:ClientSecret"]
                },
                Scopes = new[] { "https://www.googleapis.com/auth/drive" },
                DataStore = new FileDataStore(applicationName)
            });

            var credential = new UserCredential(apiCodeFlow, username, tokenResponse);

            var service = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = applicationName
            });
            return service;
        }



        //private DriveService GetService()
        //{
        //    using var stream = new FileStream("service-account.json", FileMode.Open, FileAccess.Read);
        //    var credential = GoogleCredential.FromStream(stream)
        //        .CreateScoped(DriveService.ScopeConstants.DriveFile);

        //    var service = new DriveService(new BaseClientService.Initializer()
        //    {
        //        HttpClientInitializer = credential,
        //        ApplicationName = "MovieAuth"
        //    });

        //    return service;
        //}






        public string UploadFile(Stream file, string fileName, string fileMime, string folder)
        {
            DriveService service = GetService();


            var driveFile = new Google.Apis.Drive.v3.Data.File();
            driveFile.Name = fileName;
            //driveFile.Description = fileDescription;
            driveFile.MimeType = fileMime;
            driveFile.Parents = new string[] { folder };


           var request = service.Files.Create(driveFile, file, fileMime);
            request.Fields = "id";

            var response = request.Upload();
            if (response.Status != Google.Apis.Upload.UploadStatus.Completed)
                throw response.Exception;

            return request.ResponseBody.Id;
        }






    }
}
