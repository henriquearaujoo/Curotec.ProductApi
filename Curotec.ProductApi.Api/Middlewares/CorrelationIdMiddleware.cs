namespace Curotec.ProductApi.Api.Middlewares;

public class CorrelationIdMiddleware
{
    private const string CorrelationIdHeader = "X-Correlation-ID";
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Try to get correlation ID from request or generate a new one
        var correlationId = context.Request.Headers.TryGetValue(CorrelationIdHeader, out var existing)
            ? existing.ToString()
            : Guid.NewGuid().ToString();

        // Store in context for use in logging/middleware
        context.Items[CorrelationIdHeader] = correlationId;

        // Add to response header
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[CorrelationIdHeader] = correlationId;
            return Task.CompletedTask;
        });

        await _next(context);
    }
}