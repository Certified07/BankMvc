using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankMvc.DTO.Response
{
    // Non-generic ServiceResult class (for operations that don't return data)
    public class ServiceResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new List<string>();

        public static ServiceResult SuccessResult(string message = "")
        {
            return new ServiceResult
            {
                Success = true,
                Message = message
            };
        }

        public static ServiceResult FailureResult(string message, List<string>? errors = null)
        {
            return new ServiceResult
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }

        public static ServiceResult FailureResult(string message, string error)
        {
            return new ServiceResult
            {
                Success = false,
                Message = message,
                Errors = new List<string> { error }
            };
        }
    }

    // Generic ServiceResult class (for operations that return data)
    public class ServiceResult<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new List<string>();
        public T? Data { get; set; }

        public static ServiceResult<T> SuccessResult(T? data = default, string message = "")
        {
            return new ServiceResult<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static ServiceResult<T> FailureResult(string message, List<string>? errors = null)
        {
            return new ServiceResult<T>
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }

        public static ServiceResult<T> FailureResult(string message, string error)
        {
            return new ServiceResult<T>
            {
                Success = false,
                Message = message,
                Errors = new List<string> { error }
            };
        }
    }
}