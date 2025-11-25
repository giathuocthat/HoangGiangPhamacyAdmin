using System;

namespace ThuocGiaThatAdmin.Server.Exceptions
{
    /// <summary>
    /// Base exception for all custom exceptions
    /// </summary>
    public abstract class BaseException : Exception
    {
        public int StatusCode { get; set; }

        protected BaseException(string message, int statusCode = 500) : base(message)
        {
            StatusCode = statusCode;
        }

        protected BaseException(string message, Exception innerException, int statusCode = 500) 
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }

    /// <summary>
    /// Exception for validation errors (400 Bad Request)
    /// </summary>
    public class ValidationException : BaseException
    {
        public ValidationException(string message) : base(message, 400) { }
    }

    /// <summary>
    /// Exception for not found errors (404 Not Found)
    /// </summary>
    public class NotFoundException : BaseException
    {
        public NotFoundException(string message) : base(message, 404) { }
        
        public NotFoundException(string entityName, object key) 
            : base($"{entityName} with key '{key}' was not found.", 404) { }
    }

    /// <summary>
    /// Exception for unauthorized access (401 Unauthorized)
    /// </summary>
    public class UnauthorizedException : BaseException
    {
        public UnauthorizedException(string message = "Unauthorized access") : base(message, 401) { }
    }

    /// <summary>
    /// Exception for forbidden access (403 Forbidden)
    /// </summary>
    public class ForbiddenException : BaseException
    {
        public ForbiddenException(string message = "Access forbidden") : base(message, 403) { }
    }

    /// <summary>
    /// Exception for business logic errors (400 Bad Request)
    /// </summary>
    public class BusinessException : BaseException
    {
        public BusinessException(string message) : base(message, 400) { }
    }

    /// <summary>
    /// Exception for conflict errors (409 Conflict)
    /// </summary>
    public class ConflictException : BaseException
    {
        public ConflictException(string message) : base(message, 409) { }
    }
}
