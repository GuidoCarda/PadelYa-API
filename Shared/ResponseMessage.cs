using System.Text.Json.Serialization;

namespace padelya_api.Shared
{
    /// <summary>
    /// Unified response structure for API endpoints
    /// </summary>
    /// <typeparam name="T">Type of the data being returned</typeparam>
    public class ResponseMessage<T>
    {
        /// <summary>
        /// Indicates if the operation was successful
        /// </summary>
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        /// <summary>
        /// Human-readable message describing the result
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Error code for programmatic error handling
        /// </summary>
        [JsonPropertyName("errorCode")]
        public string? ErrorCode { get; set; }

        /// <summary>
        /// The actual data payload
        /// </summary>
        [JsonPropertyName("data")]
        public T? Data { get; set; }

        /// <summary>
        /// Validation errors when applicable
        /// </summary>
        [JsonPropertyName("validationErrors")]
        public List<ValidationError>? ValidationErrors { get; set; }

        #region Success Factory Methods

        /// <summary>
        /// Creates a successful response with data
        /// </summary>
        public static ResponseMessage<T> SuccessResult(T data, string message = "Operation completed successfully")
        {
            return new ResponseMessage<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        /// <summary>
        /// Creates a successful response without data
        /// </summary>
        public static ResponseMessage<T> SuccessMessage(string message = "Operation completed successfully")
        {
            return new ResponseMessage<T>
            {
                Success = true,
                Message = message
            };
        }

        #endregion

        #region Error Factory Methods

        /// <summary>
        /// Creates an error response
        /// </summary>
        public static ResponseMessage<T> Error(string message, string? errorCode = null)
        {
            return new ResponseMessage<T>
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode
            };
        }

        /// <summary>
        /// Creates a not found response
        /// </summary>
        public static ResponseMessage<T> NotFound(string message = "Resource not found", string? errorCode = "NOT_FOUND")
        {
            return new ResponseMessage<T>
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode
            };
        }

        /// <summary>
        /// Creates a validation error response
        /// </summary>
        public static ResponseMessage<T> ValidationError(string message, List<ValidationError> validationErrors, string? errorCode = "VALIDATION_ERROR")
        {
            return new ResponseMessage<T>
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode,
                ValidationErrors = validationErrors
            };
        }

        /// <summary>
        /// Creates a validation error response from ModelState
        /// </summary>
        public static ResponseMessage<T> ValidationError(string message, Dictionary<string, string[]> modelStateErrors, string? errorCode = "VALIDATION_ERROR")
        {
            var validationErrors = modelStateErrors
                .SelectMany(kvp => kvp.Value.Select(error => new ValidationError(kvp.Key, error)))
                .ToList();

            return ValidationError(message, validationErrors, errorCode);
        }

        /// <summary>
        /// Creates an unauthorized response
        /// </summary>
        public static ResponseMessage<T> Unauthorized(string message = "Unauthorized access", string? errorCode = "UNAUTHORIZED")
        {
            return new ResponseMessage<T>
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode
            };
        }

        /// <summary>
        /// Creates a forbidden response
        /// </summary>
        public static ResponseMessage<T> Forbidden(string message = "Access forbidden", string? errorCode = "FORBIDDEN")
        {
            return new ResponseMessage<T>
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode
            };
        }

        /// <summary>
        /// Creates a conflict response
        /// </summary>
        public static ResponseMessage<T> Conflict(string message = "Resource conflict", string? errorCode = "CONFLICT")
        {
            return new ResponseMessage<T>
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode
            };
        }

        #endregion
    }

    /// <summary>
    /// Represents a validation error
    /// </summary>
    public class ValidationError
    {
        [JsonPropertyName("field")]
        public string Field { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("value")]
        public object? Value { get; set; }

        public ValidationError(string field, string message, object? value = null)
        {
            Field = field;
            Message = message;
            Value = value;
        }
    }

    /// <summary>
    /// Non-generic response for operations without return data
    /// </summary>
    public class ResponseMessage : ResponseMessage<object>
    {
        /// <summary>
        /// Creates a successful response without data
        /// </summary>
        public static new ResponseMessage SuccessMessage(string message = "Operation completed successfully")
        {
            return new ResponseMessage
            {
                Success = true,
                Message = message
            };
        }

        /// <summary>
        /// Creates an error response without data
        /// </summary>
        public static new ResponseMessage Error(string message, string? errorCode = null)
        {
            return new ResponseMessage
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode
            };
        }

        /// <summary>
        /// Creates a not found response without data
        /// </summary>
        public static new ResponseMessage NotFound(string message = "Resource not found", string? errorCode = "NOT_FOUND")
        {
            return new ResponseMessage
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode
            };
        }

        /// <summary>
        /// Creates a validation error response without data
        /// </summary>
        public static new ResponseMessage ValidationError(string message, List<ValidationError> validationErrors, string? errorCode = "VALIDATION_ERROR")
        {
            return new ResponseMessage
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode,
                ValidationErrors = validationErrors
            };
        }

        /// <summary>
        /// Creates a validation error response from ModelState without data
        /// </summary>
        public static new ResponseMessage ValidationError(string message, Dictionary<string, string[]> modelStateErrors, string? errorCode = "VALIDATION_ERROR")
        {
            var validationErrors = modelStateErrors
                .SelectMany(kvp => kvp.Value.Select(error => new ValidationError(kvp.Key, error)))
                .ToList();

            return ValidationError(message, validationErrors, errorCode);
        }
    }
}
