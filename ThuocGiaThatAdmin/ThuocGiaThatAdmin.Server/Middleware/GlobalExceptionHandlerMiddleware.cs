using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Server.Exceptions;
using ThuocGiaThatAdmin.Server.Models;

namespace ThuocGiaThatAdmin.Server.Middleware
{
    /// <summary>
    /// Global exception handling middleware
    /// Catches all unhandled exceptions and returns a standardized error response
    /// </summary>
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

        public GlobalExceptionHandlerMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionHandlerMiddleware> logger)
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
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            
            var response = new ApiResponse<object>
            {
                Success = false,
                Timestamp = DateTime.UtcNow
            };

            switch (exception)
            {
                case ValidationException validationEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = validationEx.Message;
                    _logger.LogWarning(validationEx, "Validation error occurred");
                    break;

                case NotFoundException notFoundEx:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.Message = notFoundEx.Message;
                    _logger.LogWarning(notFoundEx, "Resource not found");
                    break;

                case UnauthorizedException unauthorizedEx:
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    response.Message = unauthorizedEx.Message;
                    _logger.LogWarning(unauthorizedEx, "Unauthorized access attempt");
                    break;

                case ForbiddenException forbiddenEx:
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    response.Message = forbiddenEx.Message;
                    _logger.LogWarning(forbiddenEx, "Forbidden access attempt");
                    break;

                case BusinessException businessEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = businessEx.Message;
                    _logger.LogWarning(businessEx, "Business logic error");
                    break;

                case ConflictException conflictEx:
                    context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                    response.Message = conflictEx.Message;
                    _logger.LogWarning(conflictEx, "Conflict error");
                    break;

                case ArgumentException argEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = argEx.Message;
                    _logger.LogWarning(argEx, "Invalid argument");
                    break;

                case InvalidOperationException invalidOpEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = invalidOpEx.Message;
                    _logger.LogWarning(invalidOpEx, "Invalid operation");
                    break;

                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response.Message = "An internal server error occurred";
                    _logger.LogError(exception, "Unhandled exception occurred");
                    
                    // Only include detailed error in development
                    if (IsDevelopment())
                    {
                        response.Errors.Add(exception.Message);
                        response.Errors.Add(exception.StackTrace);
                    }
                    break;
            }

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(jsonResponse);
        }

        private bool IsDevelopment()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            return environment == "Development" || environment == "Local";
        }
    }
}
