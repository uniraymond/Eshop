using Serilog.Context;
using System.Diagnostics;

namespace Eshop.API.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context) {
            var stopwatch = Stopwatch.StartNew();

            var method = context.Request.Method;
            var path = context.Request.Path;
            var queryString = context.Request.QueryString.HasValue
                ? context.Request.QueryString.Value
                : string.Empty;

            var traceId = context.TraceIdentifier;
            var userId = context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            using (LogContext.PushProperty("TraceId", traceId))
            using (LogContext.PushProperty("RequestMethod", method))
            using (LogContext.PushProperty("RequestPath", path.ToString()))
            using (LogContext.PushProperty("UserId", userId ?? string.Empty))
            {
                _logger.LogInformation("Incoming request {Method} {Path}{QueryString}",
                    method,
                    path,
                    queryString);

                try
                {
                    await _next(context);
                }
                finally
                {
                    stopwatch.Stop();
                    var statusCode = context.Response.StatusCode;

                    _logger.LogInformation(
                        "Completed request {Method} {Path} with status code {StatusCode} in {ElapsedMilliseconds} ms",
                        method,
                        path,
                        statusCode,
                        stopwatch.ElapsedMilliseconds);
                }
            }
        }
    }
}
