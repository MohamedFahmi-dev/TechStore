using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace TechStore.API.Filters;

public class IdempotentAttribute : Attribute, IAsyncActionFilter
{
    private const string IdempotencyHeaderName = "Idempotency-Key";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(24);

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var cache = context.HttpContext.RequestServices.GetRequiredService<IDistributedCache>();

        if (!context.HttpContext.Request.Headers.TryGetValue(IdempotencyHeaderName, out var keyValues))
        {
            await next();
            return;
        }

        var key = keyValues.FirstOrDefault();
        if (string.IsNullOrEmpty(key)) { await next(); return; }

        var cacheKey = $"Idempotent:{key}";

        // Cache HIT
        var cached = await cache.GetStringAsync(cacheKey);
        if (cached != null)
        {
            var stored = JsonSerializer.Deserialize<CachedResponse>(cached)!;
            context.HttpContext.Response.StatusCode = stored.StatusCode;
            context.HttpContext.Response.ContentType = stored.ContentType;
            await context.HttpContext.Response.WriteAsync(stored.Body);
            return;
        }

        // Swap stream BEFORE execution so we can read body after
        var originalBody = context.HttpContext.Response.Body;
        using var buffer = new MemoryStream();
        context.HttpContext.Response.Body = buffer;

        var executedContext = await next();

        // Read body from buffer
        buffer.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(buffer).ReadToEndAsync();

        // Send to real client
        buffer.Seek(0, SeekOrigin.Begin);
        await buffer.CopyToAsync(originalBody);
        context.HttpContext.Response.Body = originalBody;

        if (executedContext.Exception != null || executedContext.Canceled) return;

        // Only cache successful responses
        var statusCode = context.HttpContext.Response.StatusCode;
        if (statusCode >= 200 && statusCode < 300)
        {
            var toStore = JsonSerializer.Serialize(new CachedResponse
            {
                StatusCode = statusCode,
                ContentType = context.HttpContext.Response.ContentType,
                Body = body
            });

            await cache.SetStringAsync(cacheKey, toStore, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheDuration
            });
        }
    }

    private class CachedResponse
    {
        public int StatusCode { get; set; }
        public string? ContentType { get; set; }
        public string Body { get; set; } = string.Empty;
    }
}
