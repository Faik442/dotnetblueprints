using DotnetBlueprints.SharedKernel.Exceptions;
using System.Net;
using System.Text.Json;

namespace DotnetBlueprints.Sales.Api;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BaseHttpException httpEx)
        {
            _logger.LogWarning(httpEx,
                "{ExceptionType} thrown. CorrelationId: {CorrelationId}, Message: {Message}, SourceSystem: {SourceSystem}",
                httpEx.GetType().Name,
                httpEx.CorrelationId,
                httpEx.ErrorMessage,
                httpEx.SourceSystem ?? "Unknown");

            context.Response.StatusCode = (int)httpEx.StatusCode;
            context.Response.ContentType = "application/json";

            var errorResponse = new
            {
                statusCode = httpEx.StatusCode,
                message = httpEx.ErrorMessage,
                correlationId = httpEx.CorrelationId
            };

            var json = JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var json = JsonSerializer.Serialize(new
            {
                statusCode = 500,
                message = "Beklenmeyen bir hata oluştu."
            });

            await context.Response.WriteAsync(json);
        }
    }
}

