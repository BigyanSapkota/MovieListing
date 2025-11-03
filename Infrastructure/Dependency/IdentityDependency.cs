using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Twilio.Base;



namespace Infrastructure.Dependency
{
     public static class IdentityDependency
    {

        public static IServiceCollection AddIdentityService(this IServiceCollection Services, IConfiguration configuration)
        {


            Services.AddIdentity<User, Role>()
                         .AddEntityFrameworkStores<ApplicationDbContext>()
                         .AddDefaultTokenProviders()
                         .AddApiEndpoints();



            Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
               .AddGoogle("Google", options =>
               {
                   options.ClientId = configuration["Authentication:Google:ClientId"];
                   options.ClientSecret = configuration["Authentication:Google:ClientSecret"];
                   options.CallbackPath = "/signin-google"; // must match Google redirect URI
               })
                .AddJwtBearer("Bearer", option =>
                {
                    option.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:ValidIssuer"],
                        ValidAudience = configuration["Jwt:ValidAudience"],
                        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]))
                    };

                    option.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
                    {
                        //triggered when authentication fails
                        OnChallenge = context =>
                        {
                            context.HandleResponse(); 
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";


                            var result = System.Text.Json.JsonSerializer.Serialize(new
                            {
                                //    statusCode = 401,
                                //    error = "Unauthorized",
                                //    message = "You must be logged in to access this resource.",
                                //    timestamp = DateTime.UtcNow

                                StatusCode = 401,
                                Description = "Unauthorized",
                                Errors = (object?) null,
                                Path = context.Request.Path,
                                Timestamp = DateTime.UtcNow
                            });

                            return context.Response.WriteAsync(result);
                        },
                        //triggered when authorization fails
                        OnForbidden = context =>
                        {
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            context.Response.ContentType = "application/json";

                            var result = System.Text.Json.JsonSerializer.Serialize(new
                            {
                                //statusCode = 403,
                                //error = "Forbidden",
                                //message = "You don’t have permission to perform this action.",
                                //timestamp = DateTime.UtcNow

                                StatusCode = 403,
                                Description = "Forbidden",
                                Errors = (object?) null,
                                Path = context.Request.Path,
                                Timestamp = DateTime.UtcNow

                            });

                            
                            return context.Response.WriteAsync(result);
                        }

                    };                  
                });
            return Services;
        }
         
    }
}
