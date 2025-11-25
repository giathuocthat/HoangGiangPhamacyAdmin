using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using ThuocGiaThatAdmin.Server.Exceptions;
using ThuocGiaThatAdmin.Server.Models;

namespace ThuocGiaThatAdmin.Server.Controllers
{
    /// <summary>
    /// Base controller with common functionality for all API controllers
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
        protected readonly ILogger Logger;

        protected BaseApiController(ILogger logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region Success Responses

        /// <summary>
        /// Return a successful response with data
        /// </summary>
        protected IActionResult Success<T>(T data, string message = "Success")
        {
            var response = ApiResponse<T>.SuccessResponse(data, message);
            return Ok(response);
        }

        /// <summary>
        /// Return a successful response without data
        /// </summary>
        protected IActionResult Success(string message = "Success")
        {
            var response = ApiResponse<object>.SuccessResponse(null, message);
            return Ok(response);
        }

        /// <summary>
        /// Return a successful paginated response
        /// </summary>
        protected IActionResult SuccessPaginated<T>(
            IEnumerable<T> data,
            int pageNumber,
            int pageSize,
            int totalCount,
            string message = "Success")
        {
            var response = PaginatedResponse<T>.SuccessPaginatedResponse(
                data, pageNumber, pageSize, totalCount, message);
            return Ok(response);
        }

        /// <summary>
        /// Return a created response (201)
        /// </summary>
        protected IActionResult Created<T>(T data, string message = "Resource created successfully")
        {
            var response = ApiResponse<T>.SuccessResponse(data, message);
            return StatusCode(201, response);
        }

        #endregion

        #region Error Responses

        /// <summary>
        /// Return a bad request response (400)
        /// </summary>
        protected IActionResult BadRequestResponse(string message, List<string> errors = null)
        {
            var response = ApiResponse<object>.ErrorResponse(message, errors);
            return BadRequest(response);
        }

        /// <summary>
        /// Return a bad request response with a single error
        /// </summary>
        protected IActionResult BadRequestResponse(string message, string error)
        {
            var response = ApiResponse<object>.ErrorResponse(message, error);
            return BadRequest(response);
        }

        /// <summary>
        /// Return a not found response (404)
        /// </summary>
        protected IActionResult NotFoundResponse(string message)
        {
            var response = ApiResponse<object>.ErrorResponse(message);
            return NotFound(response);
        }

        /// <summary>
        /// Return an unauthorized response (401)
        /// </summary>
        protected IActionResult UnauthorizedResponse(string message = "Unauthorized access")
        {
            var response = ApiResponse<object>.ErrorResponse(message);
            return Unauthorized(response);
        }

        /// <summary>
        /// Return a forbidden response (403)
        /// </summary>
        protected IActionResult ForbiddenResponse(string message = "Access forbidden")
        {
            var response = ApiResponse<object>.ErrorResponse(message);
            return StatusCode(403, response);
        }

        /// <summary>
        /// Return a conflict response (409)
        /// </summary>
        protected IActionResult ConflictResponse(string message)
        {
            var response = ApiResponse<object>.ErrorResponse(message);
            return Conflict(response);
        }

        /// <summary>
        /// Return an internal server error response (500)
        /// </summary>
        protected IActionResult InternalServerErrorResponse(string message = "An internal server error occurred")
        {
            var response = ApiResponse<object>.ErrorResponse(message);
            return StatusCode(500, response);
        }

        #endregion

        #region Validation

        /// <summary>
        /// Validate model state and return bad request if invalid
        /// </summary>
        protected IActionResult ValidateModelState()
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequestResponse("Validation failed", errors);
            }

            return null;
        }

        /// <summary>
        /// Execute an action with automatic exception handling
        /// </summary>
        protected IActionResult ExecuteAction(Func<IActionResult> action, string operationName = "Operation")
        {
            try
            {
                // Validate model state first
                var validationResult = ValidateModelState();
                if (validationResult != null)
                    return validationResult;

                return action();
            }
            catch (ValidationException ex)
            {
                Logger.LogWarning(ex, $"{operationName} validation failed: {ex.Message}");
                return BadRequestResponse(ex.Message);
            }
            catch (NotFoundException ex)
            {
                Logger.LogWarning(ex, $"{operationName} - Resource not found: {ex.Message}");
                return NotFoundResponse(ex.Message);
            }
            catch (UnauthorizedException ex)
            {
                Logger.LogWarning(ex, $"{operationName} - Unauthorized: {ex.Message}");
                return UnauthorizedResponse(ex.Message);
            }
            catch (ForbiddenException ex)
            {
                Logger.LogWarning(ex, $"{operationName} - Forbidden: {ex.Message}");
                return ForbiddenResponse(ex.Message);
            }
            catch (BusinessException ex)
            {
                Logger.LogWarning(ex, $"{operationName} - Business logic error: {ex.Message}");
                return BadRequestResponse(ex.Message);
            }
            catch (ConflictException ex)
            {
                Logger.LogWarning(ex, $"{operationName} - Conflict: {ex.Message}");
                return ConflictResponse(ex.Message);
            }
            catch (ArgumentException ex)
            {
                Logger.LogWarning(ex, $"{operationName} - Invalid argument: {ex.Message}");
                return BadRequestResponse(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                Logger.LogWarning(ex, $"{operationName} - Invalid operation: {ex.Message}");
                return BadRequestResponse(ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"{operationName} failed with unexpected error");
                return InternalServerErrorResponse($"An error occurred while processing {operationName.ToLower()}");
            }
        }

        /// <summary>
        /// Execute an async action with automatic exception handling
        /// </summary>
        protected async System.Threading.Tasks.Task<IActionResult> ExecuteActionAsync(
            Func<System.Threading.Tasks.Task<IActionResult>> action, 
            string operationName = "Operation")
        {
            try
            {
                // Validate model state first
                var validationResult = ValidateModelState();
                if (validationResult != null)
                    return validationResult;

                return await action();
            }
            catch (ValidationException ex)
            {
                Logger.LogWarning(ex, $"{operationName} validation failed: {ex.Message}");
                return BadRequestResponse(ex.Message);
            }
            catch (NotFoundException ex)
            {
                Logger.LogWarning(ex, $"{operationName} - Resource not found: {ex.Message}");
                return NotFoundResponse(ex.Message);
            }
            catch (UnauthorizedException ex)
            {
                Logger.LogWarning(ex, $"{operationName} - Unauthorized: {ex.Message}");
                return UnauthorizedResponse(ex.Message);
            }
            catch (ForbiddenException ex)
            {
                Logger.LogWarning(ex, $"{operationName} - Forbidden: {ex.Message}");
                return ForbiddenResponse(ex.Message);
            }
            catch (BusinessException ex)
            {
                Logger.LogWarning(ex, $"{operationName} - Business logic error: {ex.Message}");
                return BadRequestResponse(ex.Message);
            }
            catch (ConflictException ex)
            {
                Logger.LogWarning(ex, $"{operationName} - Conflict: {ex.Message}");
                return ConflictResponse(ex.Message);
            }
            catch (ArgumentException ex)
            {
                Logger.LogWarning(ex, $"{operationName} - Invalid argument: {ex.Message}");
                return BadRequestResponse(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                Logger.LogWarning(ex, $"{operationName} - Invalid operation: {ex.Message}");
                return BadRequestResponse(ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"{operationName} failed with unexpected error");
                return InternalServerErrorResponse($"An error occurred while processing {operationName.ToLower()}");
            }
        }

        #endregion
    }
}
