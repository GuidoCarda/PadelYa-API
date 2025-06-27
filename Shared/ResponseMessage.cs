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

        /// <summary>
        /// Pagination information when applicable
        /// </summary>
        [JsonPropertyName("pagination")]
        public PaginationInfo? Pagination { get; set; }

        /// <summary>
        /// Metadata about the response (pagination, timestamps, etc.)
        /// </summary>
        [JsonPropertyName("metadata")]
        public ResponseMetadata? Metadata { get; set; }

        /// <summary>
        /// Timestamp when the response was generated
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Request correlation ID for tracking
        /// </summary>
        [JsonPropertyName("correlationId")]
        public string? CorrelationId { get; set; }

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

        /// <summary>
        /// Creates a successful response with pagination
        /// </summary>
        public static ResponseMessage<T> SuccessResult(T data, PaginationInfo pagination, string message = "Data retrieved successfully")
        {
            return new ResponseMessage<T>
            {
                Success = true,
                Message = message,
                Data = data,
                Pagination = pagination
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

        #region Utility Methods

        /// <summary>
        /// Sets the correlation ID for request tracking
        /// </summary>
        public ResponseMessage<T> WithCorrelationId(string correlationId)
        {
            CorrelationId = correlationId;
            return this;
        }

        /// <summary>
        /// Sets metadata for the response
        /// </summary>
        public ResponseMessage<T> WithMetadata(ResponseMetadata metadata)
        {
            Metadata = metadata;
            return this;
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
    /// Metadata about the response
    /// </summary>
    public class ResponseMetadata
    {
        [JsonPropertyName("pagination")]
        public PaginationInfo? Pagination { get; set; }

        [JsonPropertyName("processingTime")]
        public TimeSpan? ProcessingTime { get; set; }

        [JsonPropertyName("serverInfo")]
        public ServerInfo? ServerInfo { get; set; }
    }

    /// <summary>
    /// Pagination information
    /// </summary>
    public class PaginationInfo
    {
        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }

        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }

        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("hasNext")]
        public bool HasNext { get; set; }

        [JsonPropertyName("hasPrevious")]
        public bool HasPrevious { get; set; }

        public PaginationInfo(int page, int pageSize, int totalCount)
        {
            Page = page;
            PageSize = pageSize;
            TotalCount = totalCount;
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            HasNext = page < TotalPages;
            HasPrevious = page > 1;
        }
    }

    /// <summary>
    /// Server information
    /// </summary>
    public class ServerInfo
    {
        [JsonPropertyName("version")]
        public string Version { get; set; } = string.Empty;

        [JsonPropertyName("environment")]
        public string Environment { get; set; } = string.Empty;

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
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
