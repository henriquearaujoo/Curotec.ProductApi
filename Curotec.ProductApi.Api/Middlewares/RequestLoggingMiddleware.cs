﻿using System.Diagnostics;

namespace Curotec.ProductApi.Api.Middlewares;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        _logger.LogInformation("Handling request: {Method} {Path}", context.Request.Method, context.Request.Path);

        await _next(context); // Call the next middleware

        stopwatch.Stop();

        _logger.LogInformation("Finished handling request: {StatusCode} in {ElapsedMilliseconds}ms",
            context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
    }
}