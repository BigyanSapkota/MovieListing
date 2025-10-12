using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interface.Repository;
using Application.Interface.Services;
using Domain.Entities;
using Hangfire;
using Infrastructure.Data;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repositories
{
     public class MovieNotificationJob : IMovieNotificationJob
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public MovieNotificationJob(ApplicationDbContext context, IEmailService emailService,IConfiguration configuration)
        {
            _context = context;
            _emailService = emailService;
            _configuration = configuration;
        }



        public void ScheduleMovieNotifications(Movie movie)
        {
            var users = _context.Users.Where(u => !string.IsNullOrEmpty(u.Email)).ToList();

            foreach (var user in users)
            {
                ScheduleEmail(user.Email, movie, -2, $"{movie.Title} is releasing in 2 days!");
                ScheduleEmail(user.Email, movie, -1, $"{movie.Title} is releasing tomorrow!");
                ScheduleEmail(user.Email, movie, 0, $"{movie.Title} is releasing today!");
            }
        }


        private void ScheduleEmail(string email, Movie movie, int daysOffset, string subject)
        {
            var senderName = _configuration["EmailSettings:SenderName"];
            var senderEmail = _configuration["EmailSettings:SenderEmail"];


            var sendTime = movie.ReleaseDate.AddDays(daysOffset);
            if (sendTime > DateTime.UtcNow)
            {
                BackgroundJob.Schedule(
                    () => _emailService.SendEmail(
                        senderName,
                        senderEmail,
                        email,
                        email,
                        subject,
                        $"Don't miss it! {movie.Title} will release on {movie.ReleaseDate:d}."
                    ),
                    sendTime
                );
            }
        }

      
    }
}
