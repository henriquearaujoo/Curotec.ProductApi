namespace Curotec.ProductApi.Api.Models;

public class ApiErrorResponse
{
    public int Status { get; init; }
    public string Message { get; init; } = "An error occurred.";
    public string? CorrelationId { get; init; }
    public List<string>? Errors { get; init; }
}