using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Net;
using WebAPI.Middlewares;


namespace WebAPI.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {

        public static void ConfigureExceptionHandler(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
        public static void ConfigureBuiltinExceptionHandler(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseExceptionHandler(options =>
            {
                options.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (exceptionFeature != null)
                    {
                        var errorResponse = new
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = env.IsDevelopment()
                                ? exceptionFeature.Error.Message
                                : "An unexpected error occurred. Please try again later."
                        };

                        var errorJson = JsonConvert.SerializeObject(errorResponse);
                        await context.Response.WriteAsync(errorJson);
                    }
                });
            });
        }
    }
}
