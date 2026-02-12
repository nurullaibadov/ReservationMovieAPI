using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationMovieAPI.Application.Common.Behaviors
{
    public class PerformanceBehavior<TRequest, TResponse>
      : IPipelineBehavior<TRequest, TResponse>
      where TRequest : notnull
    {
        private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
        private const int WarningThresholdMs = 500;

        public PerformanceBehavior(
            ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = await next();
            stopwatch.Stop();

            if (stopwatch.ElapsedMilliseconds > WarningThresholdMs)
            {
                _logger.LogWarning(
                    "SLOW QUERY DETECTED: {RequestName} took {ElapsedMs}ms. " +
                    "Request: {@Request}",
                    typeof(TRequest).Name,
                    stopwatch.ElapsedMilliseconds,
                    request);
            }

            return response;
        }
    }
    }
