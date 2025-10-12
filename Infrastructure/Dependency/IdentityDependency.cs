using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;



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
                });

            return Services;
        }

    }
}
