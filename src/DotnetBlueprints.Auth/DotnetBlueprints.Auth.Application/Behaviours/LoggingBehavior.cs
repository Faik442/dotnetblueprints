using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.Auth.Application.Behaviours;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {RequestName} {@Request}", typeof(TRequest).Name, request);

        try
        {
            var response = await next();

            _logger.LogInformation("Handled {RequestName} {@Response}", typeof(TRequest).Name, response);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in {RequestName}: {@Request}", typeof(TRequest).Name, request);
            throw;
        }
    }
}
