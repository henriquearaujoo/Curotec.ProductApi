using Curotec.ProductApi.Api.Models;
using System.Net;
using System.Text.Json;
using FluentValidation;

namespace Curotec.ProductApi.Api.Middlewares;

public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private const string CorrelationIdHeader = "X-Correlation-ID";

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                    await _next(context);
            }
            catch (ValidationException validationEx)
            {
                await HandleValidationExceptionAsync(context, validationEx);
            }
            catch (Exception ex)
            {
                await HandleUnhandledExceptionAsync(context, ex);
            }
        }

        private async Task HandleValidationExceptionAsync(HttpContext context, ValidationException ex)
        {
            var correlationId = context.Items.TryGetValue(CorrelationIdHeader, out var cid)
                ? cid?.ToString()
                : null;

            _logger.LogWarning("Validation failed. CorrelationId: {CorrelationId}. Errors: {@Errors}",
                correlationId, ex.Errors.Select(e => e.ErrorMessage));

            var errorResponse = new ApiErrorResponse
            {
                Status = (int)HttpStatusCode.BadRequest,
                Message = "Validation failed.",
                CorrelationId = correlationId,
                Errors = ex.Errors.Select(e => e.ErrorMessage).ToList()
            };

            context.Response.StatusCode = errorResponse.Status;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }

        private async Task HandleUnhandledExceptionAsync(HttpContext context, Exception ex)
        {
            var correlationId = context.Items.TryGetValue(CorrelationIdHeader, out var cid)
                ? cid?.ToString()
                : null;

            _logger.LogError(ex, "Unhandled exception occurred. CorrelationId: {CorrelationId}", correlationId);

            var errorResponse = new ApiErrorResponse
            {
                Status = (int)HttpStatusCode.InternalServerError,
                Message = "An unexpected error occurred.",
                CorrelationId = correlationId
            };

            context.Response.StatusCode = errorResponse.Status;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }
    }