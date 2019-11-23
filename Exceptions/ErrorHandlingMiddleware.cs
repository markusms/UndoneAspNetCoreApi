using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace UndoneAspNetCoreApi.Exceptions
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await next(httpContext);
            }
            catch (NotFoundException ex)
            {
                Console.WriteLine(ex);
                httpContext.Response.StatusCode = 404;
            }
        }
    }
}
