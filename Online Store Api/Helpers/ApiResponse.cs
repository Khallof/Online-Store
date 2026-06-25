using System.Collections.Generic;

namespace Store.API.Helpers
{
    // ==================================================
    // Standard API Response Wrapper
    // Every endpoint returns this format
    // ==================================================
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public int StatusCode { get; set; }
        public List<string> Errors { get; set; } = new();

        // 1. Success Response
        public static ApiResponse<T> Ok(T data, string message = "Success", int statusCode = 200)
        {
            return new ApiResponse<T>
            {
                Success = true,
                StatusCode = statusCode,
                Message = message,
                Data = data,
                Errors = new List<string>()
            };
        }

        // 2. Generic Failure Response
        public static ApiResponse<T> Fail(string message, List<string>? errors = null, int statusCode = 400)
        {
            return new ApiResponse<T>
            {
                Success = false,
                StatusCode = statusCode,
                Message = message,
                Data = default,
                Errors = errors ?? new List<string>()
            };
        }

        // 3. Validation Error Response (400 Bad Request)
        public static ApiResponse<T> ValidationError(List<string> errors, string message = "Validation failed")
        {
            return new ApiResponse<T>
            {
                Success = false,
                StatusCode = 400,
                Message = message,
                Data = default,
                Errors = errors
            };
        }

        // 4. Not Found Response (404)
        public static ApiResponse<T> NotFound(string message = "Resource not found")
        {
            return new ApiResponse<T>
            {
                Success = false,
                StatusCode = 404,
                Message = message,
                Data = default,
                Errors = new List<string> { message }
            };
        }

        // 5. Unauthorized Response (401)
        public static ApiResponse<T> Unauthorized(string message = "Unauthorized access")
        {
            return new ApiResponse<T>
            {
                Success = false,
                StatusCode = 401,
                Message = message,
                Data = default,
                Errors = new List<string> { message }
            };
        }

        // 6. Forbidden Response (403)
        public static ApiResponse<T> Forbidden(string message = "Access forbidden")
        {
            return new ApiResponse<T>
            {
                Success = false,
                StatusCode = 403,
                Message = message,
                Data = default,
                Errors = new List<string> { message }
            };
        }
    }
}