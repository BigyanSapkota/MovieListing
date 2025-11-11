using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interface;
using Application.Interface.Repository;
using Application.Interface.Services;
using Application.Service;
using Infrastructure.Repositories;
using Infrastructure.Service;
using Microsoft.Extensions.DependencyInjection;
using Shared.Helper;

namespace Infrastructure.Dependency
{
     public static class AddInfDependency
    {

        public static IServiceCollection AddInfrastructureService(this IServiceCollection Services)
        {

            Services.AddScoped<IGoogleAuthRepo, GoogleAuthRepo>();

            Services.AddHttpClient<GrokAIService>();
            Services.AddScoped<GroqService>();


            Services.AddScoped<FirebaseService>();
            Services.AddScoped<IDeviceTokenRepo, DeviceTokenRepo>();
            Services.AddScoped<IPushNotificationService, PushNotificationService>();




            //Services.AddHttpClient<IGrokService, GrokService>();
            Services.AddSingleton<ClaudeService>();

            Services.AddHttpClient<OpenAIService>();
            Services.AddSingleton<OpenAIService>();


            Services.AddHttpClient<IZeroBounceService, ZeroBounceService>();
            Services.AddScoped<IZeroBounceService, ZeroBounceService>();

            Services.AddScoped(typeof(IGenericRepo<,>), typeof(GenericRepo<,>));


            Services.AddScoped<IUnitOfWork, UnitOfWork>();
            Services.AddScoped<EmailHelper>();
            Services.AddScoped<UserInfoHelper>();
            Services.AddScoped<GenerateOtpHelper>();

            Services.AddScoped<IAuthService, AuthService>();
            Services.AddScoped<IMovieRepo, MovieRepo>();
            Services.AddScoped<IMovieService, MovieService>();
            Services.AddScoped<IMovieRatingService, MovieRatingService>();
            Services.AddScoped<IMovieRatingRepo, MovieRatingRepo>();
            Services.AddScoped<IMovieCommentService, MovieCommentService>();
            Services.AddScoped<IMovieCommentRepo, MovieCommentRepo>();
            Services.AddScoped<IGenreService, GenreService>();
            Services.AddScoped<IGenreRepo, GenreRepo>();
            Services.AddScoped<ILanguageService, LanguageService>();
            Services.AddScoped<ILanguageRepo, LanguageRepo>();
            Services.AddScoped<IEmailService, EmailService>();
            Services.AddScoped<IMovieNotificationJob, MovieNotificationJob>();


            Services.AddScoped<IWatchListService, WatchListService>();
            Services.AddScoped<IWatchListRepo, WatchListRepo>();

            Services.AddScoped<IPaymentRepo, PaymentRepo>();

            Services.AddScoped<IJobService, JobService>();


            //Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            Services.AddAutoMapper(typeof(Application.MappingProfile));

            Services.AddScoped<IBillRepo, BillRepo>();
            Services.AddScoped<IBillService, BillService>();

            Services.AddScoped<IBillTypeRepo, BillTypeRepo>();
           

            Services.AddScoped<IKhaltiPaymentService, KhaltiPaymentService>();
            Services.AddScoped<IEsewaPaymentService, EsewaPaymentService>();
            Services.AddScoped<IFonePayService, FonePayService>();



            Services.AddSingleton<UploadToDrive>();
            //Services.AddScoped<ValidateEmailHelper>();
            Services.AddScoped<ValidateEmail>();

            Services.AddHttpClient<IGoogleAuthService, GoogleAuthService>(client =>
            {
                client.BaseAddress = new Uri("https://www.googleapis.com/oauth2/v3/");
            });


            Services.AddScoped<ISMSSenderService, SMSSenderService>();
            Services.AddScoped<ISMSRepo, SMSRepo>();

            Services.AddScoped<VonageSMSService>();

            Services.AddScoped<IDeleteRequestRepo, DeleteRequestRepo>();
            Services.AddScoped<IDeleteRequestService, DeleteRequestService>();

            Services.AddScoped<IOrganizationService, OrganizationService>();
            Services.AddScoped<IOrganizationRepo, OrganizationRepo>();

            Services.AddScoped<IPdfService, PdfService>();
            Services.AddScoped<IPdfGenerateService, PdfGenerateService>();


            return Services;

        }

    }
}
