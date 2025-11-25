using System;
using System.Collections.Generic;

namespace ThuocGiaThatAdmin.Server.Models
{
    /// <summary>
    /// Standard API response wrapper
    /// </summary>
    /// <typeparam name="T">Type of data being returned</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Indicates if the request was successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Response message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Response data
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// List of errors (if any)
        /// </summary>
        public List<string> Errors { get; set; }

        /// <summary>
        /// Timestamp of the response
        /// </summary>
        public DateTime Timestamp { get; set; }

        public ApiResponse()
        {
            Timestamp = DateTime.UtcNow;
            Errors = new List<string>();
        }

        /// <summary>
        /// Create a successful response
        /// </summary>
        public static ApiResponse<T> SuccessResponse(T data, string message = "Success")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        /// <summary>
        /// Create an error response
        /// </summary>
        public static ApiResponse<T> ErrorResponse(string message, List<string> errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }

        /// <summary>
        /// Create an error response with a single error
        /// </summary>
        public static ApiResponse<T> ErrorResponse(string message, string error)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Errors = new List<string> { error }
            };
        }
    }

    /// <summary>
    /// Paginated API response
    /// </summary>
    public class PaginatedResponse<T> : ApiResponse<IEnumerable<T>>
    {
        /// <summary>
        /// Pagination metadata
        /// </summary>
        public PaginationMetadata Pagination { get; set; }

        /// <summary>
        /// Create a successful paginated response
        /// </summary>
        public static PaginatedResponse<T> SuccessPaginatedResponse(
            IEnumerable<T> data, 
            int pageNumber, 
            int pageSize, 
            int totalCount,
            string message = "Success")
        {
            return new PaginatedResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                Pagination = new PaginationMetadata
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                }
            };
        }
    }

    /// <summary>
    /// Pagination metadata
    /// </summary>
    public class PaginationMetadata
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasPrevious => PageNumber > 1;
        public bool HasNext => PageNumber < TotalPages;
    }
}
