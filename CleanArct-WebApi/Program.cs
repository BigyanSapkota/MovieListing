using System.Net;
using System.Text.Json;
using Application.Interface;
using Application.Interface.Repository;
using Application.Interface.Services;
using Application.Service;
using CleanArct_WebApi.Exception;
using Domain.Entities;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Hangfire;
using Infrastructure;
using Infrastructure.Data;
using Infrastructure.Dependency;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Shared.Helper;
using static CleanArct_WebApi.Exception.ExceptionHandling;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});



// Serilog Configuration
Log.Logger = new LoggerConfiguration()
              .ReadFrom.Configuration(builder.Configuration)
              .WriteTo.Console()
              .WriteTo.File("logs/MyAppLog.txt", rollingInterval: RollingInterval.Day)
              .CreateLogger();

builder.Host.UseSerilog();






// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();








builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "Jwt",
        Scheme = "Bearer"
    });


    options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
            }
        });
});



builder.Services.AddAuthorization();

builder.Services.AddHttpContextAccessor();





builder.Services.AddHangfire(configuration =>
{
    configuration.UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddHangfireServer();







builder.Services.AddHttpContextAccessor();



// Load Firebase Service Account JSON
FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile(Path.Combine(AppContext.BaseDirectory, "wwwroot", "serviceAccountKey.json"))
});


////////////////////////////////////////////////////////////////////////////

builder.Services.AddIdentityService(builder.Configuration);

builder.Services.AddInfrastructureService();



builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(x => x.Value.Errors.Count > 0)
            .ToDictionary(
                x => x.Key,
                x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            );

        var response = new ExceptionResponse
        {
            StatusCode = StatusCodes.Status400BadRequest,
            Description = "Error Occurs",
            Errors = errors,
            Path = context.HttpContext.Request.Path,
            Timestamp = DateTime.UtcNow
        };

        return new BadRequestObjectResult(response);
    };
});







var app = builder.Build();







// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();



//app.UseExceptionHandler(errorApp =>
//{
//    errorApp.Run(async context =>
//    {
//        context.Response.ContentType = "application/json";
//        var feature = context.Features.Get<IExceptionHandlerFeature>();    // Retrieves the exception information

//        if (feature != null)
//        {
//            var ex = feature.Error;        
//            int statusCode = (int)HttpStatusCode.InternalServerError;

//            if (ex is UnauthorizedAccessException)
//                statusCode = (int)HttpStatusCode.Unauthorized; 
//            else if (ex is ArgumentException)
//                statusCode = (int)HttpStatusCode.BadRequest;   
//            else if (ex is KeyNotFoundException)
//                statusCode = (int)HttpStatusCode.NotFound;     
//            else if (ex is NotImplementedException)
//                statusCode = (int)HttpStatusCode.NotImplemented; 

//            context.Response.StatusCode = statusCode;

//            var result = JsonSerializer.Serialize(new
//            {
//                StatusCode = statusCode,
//                Message = ex.Message,
//                ErrorType = ex.GetType().Name,
//                Path = context.Request.Path
//            });

//            await context.Response.WriteAsync(result);
//        }
//    });
//});








app.UseMiddleware<ExceptionHandling>();

app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers();

app.MapFallbackToFile("index.html");


using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

    string[] roleNames = { "Admin", "User"};

    foreach (var roleName in roleNames)
    {
        var roleExists = await roleManager.RoleExistsAsync(roleName);
        if (!roleExists)
        {
            await roleManager.CreateAsync(new Role(roleName));
        }
    }
}




app.UseHangfireDashboard();



app.Run();
