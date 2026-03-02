using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace AskErik.Api
{
    public class RequestLoggingFilter : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            // Example logging logic
            var request = context.HttpContext.Request;
            Console.WriteLine($"Request: {request.Method} {request.Path}");

            return await next(context);
        }
    }
}