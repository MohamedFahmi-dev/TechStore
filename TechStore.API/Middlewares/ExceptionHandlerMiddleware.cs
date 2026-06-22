using Microsoft.AspNetCore.Mvc;

namespace TechStore.API.Middlewares;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next.Invoke(httpContext);
            await HandleNotFoundEndpointAsync(httpContext);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Something Went Wrong");
            var problem = new ProblemDetails
            {
                Title = "Error While Processing HTTP Request",
                Detail = "An unexpected error occurred.", // Masking the actual message for security as per the report
                Instance = httpContext.Request.Path,
                Status = StatusCodes.Status500InternalServerError
            };
            httpContext.Response.StatusCode = problem.Status.Value;
            await httpContext.Response.WriteAsJsonAsync(problem);
        }
    }

    private static async Task HandleNotFoundEndpointAsync(HttpContext httpContext)
    {
        if (httpContext.Response.StatusCode == StatusCodes.Status404NotFound && !httpContext.Response.HasStarted)
        {
            var problem = new ProblemDetails
            {
                Title = "Error While Processing the HTTP Request - Endpoint Not Found",
                Detail = $"Endpoint {httpContext.Request.Path} Not Found",
                Status = StatusCodes.Status404NotFound,
                Instance = httpContext.Request.Path
            };
            await httpContext.Response.WriteAsJsonAsync(problem);
        }
    }
}
