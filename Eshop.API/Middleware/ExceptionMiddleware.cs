using Eshop.Application.Common.Exceptions;
using Eshop.Application.Common.Models;
using System.Net;
using System.Text.Json;

namespace Eshop.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred while processing the request.");
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext httpContext, Exception ex)
        {
            var response = new ErrorResponse
            {
                Success = false,
                TraceId = httpContext.TraceIdentifier,
            };

            switch (ex)
            {
                case ValidationException validationException:
                    httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = validationException.Message;
                    response.Errors = validationException.Errors;
                    break;

                case UnauthorizedException unauthorizedException:
                    httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    response.Message = unauthorizedException.Message;
                    break;

                case NotFoundException notFoundException:
                    httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.Message = notFoundException.Message;
                    break;

                case BussinessException bussinessException:
                    httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = bussinessException.Message;
                    break;
                default:
                    httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response.Message = "An unexpected error occurred. Please try again later.";
                    break;
            }

            httpContext.Response.ContentType = "application/json";

            var json = JsonSerializer.Serialize(response);

            await httpContext.Response.WriteAsync(json);
        }
    }
}
