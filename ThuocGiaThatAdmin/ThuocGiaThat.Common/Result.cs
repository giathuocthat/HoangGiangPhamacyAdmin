using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThuocGiaThatAdmin.Common
{
    public class Result
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new();

        public static Result Success(string message = "Operation successful")
            => new() { IsSuccess = true, Message = message };

        public static Result Failure(params string[] errors)
            => new() { IsSuccess = false, Errors = new List<string>(errors) };
    }

    public class Result<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new();

        public static Result<T> Success(T data, string message = "Operation successful")
            => new() { IsSuccess = true, Data = data, Message = message };

        public static Result<T> Failure(params string[] errors)
            => new() { IsSuccess = false, Errors = new List<string>(errors) };
    }
}
