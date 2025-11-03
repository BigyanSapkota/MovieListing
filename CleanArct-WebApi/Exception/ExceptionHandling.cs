using System.Net;

namespace CleanArct_WebApi.Exception
{
    public class ExceptionHandling
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandling> _logger;
        public ExceptionHandling(RequestDelegate next, ILogger<ExceptionHandling> logger)
        {
            _next = next;
            _logger = logger;
        }

        //public async Task InvokeAsync(HttpContext context)
        //{
        //    try
        //    {
        //        await _next(context);
        //        if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
        //        {
        //            context.Response.ContentType = "application/json";
        //            var response = new ExceptionResponse
        //            {
        //                StatusCode = 401,
        //                Description = "Unauthorized",
        //                Errors = null,
        //                Path = context.Request.Path,
        //                Timestamp = DateTime.UtcNow
        //            };
        //            await context.Response.WriteAsJsonAsync(response);
        //        }
        //    }
        //    catch (System.Exception exception) 
        //    {
        //        await HandleExceptionAsync(context, exception);
        //    }
        //}



        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); 

            }
            catch (System.Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }




        private async Task HandleExceptionAsync(HttpContext context, System.Exception exception) 
        {
            var statusCode = GetStatusCode(exception);
            var response = new ExceptionResponse
            {
                StatusCode = (int)GetStatusCode(exception),
                Description = statusCode.ToString(),
                Errors = GetErrorMessage(exception),
                Path = context.Request.Path,
                Timestamp = DateTime.Now
            };
            if(!context.Response.HasStarted)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)response.StatusCode;
                await context.Response.WriteAsJsonAsync(response);
            }
            
        }


        private static HttpStatusCode GetStatusCode(System.Exception exception) => exception switch
        {
            KeyNotFoundException => HttpStatusCode.NotFound,
            UnauthorizedAccessException => HttpStatusCode.Unauthorized,
            ArgumentException => HttpStatusCode.BadRequest,
            _ => HttpStatusCode.InternalServerError
        };
        private static string GetErrorMessage(System.Exception exception) => exception switch
        {
            KeyNotFoundException => exception.Message,
            UnauthorizedAccessException => "You are not authorized to access this resource.",
            ArgumentException => exception.Message,
            _ => "An unexpected error occurred on the server."
        };



        //public record ExceptionResponse(HttpStatusCode StatusCode, string Description);
        public record ExceptionResponse
        {
            public int StatusCode { get; init; }
            public string Description { get; init; }
            public object? Errors { get; init; }
            public string? Path { get; init; }
            public DateTime Timestamp { get; init; }

        }

    }
}
