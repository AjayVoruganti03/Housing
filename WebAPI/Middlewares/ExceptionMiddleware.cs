using System.Net;
using System.Text.Json;
using WebAPI.Errors;

namespace WebAPI.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next,
                                   ILogger<ExceptionMiddleware> logger,
                                   IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                ApiError response;
                HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
                string message;
                var exceptionType = ex.GetType();

                if(exceptionType == typeof(ArgumentNullException))
                {
                    statusCode = HttpStatusCode.BadRequest;
                    message = "Argument cannot be null";
                }
                else if (exceptionType == typeof(ArgumentException))
                {
                    statusCode = HttpStatusCode.BadRequest;
                    message = "Invalid argument provided";
                }
                else if (exceptionType == typeof(UnauthorizedAccessException))
                {
                    statusCode = HttpStatusCode.Unauthorized;
                    message = "Unauthorized access";
                }
                else if (exceptionType == typeof(InvalidOperationException))
                {
                    statusCode = HttpStatusCode.Conflict;
                    message = "Invalid operation";
                }
                else
                {
                    message = "An unexpected error occurred";
                }

                if (_env.IsDevelopment())
                {
                    response = new ApiError((int)statusCode, ex.Message, ex.StackTrace?.ToString());
                }
                else
                {
                    response = new ApiError((int)statusCode, "Internal Server Error");
                }

                _logger.LogError(ex, ex.Message);
                context.Response.StatusCode = (int)statusCode;
                context.Response.ContentType = "application/json";

                var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                await context.Response.WriteAsync(json);
            }
        }
    }
}
