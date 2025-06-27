using System.Net.Mail;

namespace padelya_api.Shared
{
    /// <summary>
    /// Simple validation helper for university project
    /// </summary>
    public static class ValidationHelper
    {
        /// <summary>
        /// Simple email validation
        /// </summary>
        /// <param name="email">Email to validate</param>
        /// <returns>True if valid email format</returns>
        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Validates string length
        /// </summary>
        /// <param name="value">String to validate</param>
        /// <param name="maxLength">Maximum allowed length</param>
        /// <param name="fieldName">Name of the field for error message</param>
        /// <returns>Validation error if invalid, null if valid</returns>
        public static ValidationError? ValidateStringLength(string? value, int maxLength, string fieldName)
        {
            if (!string.IsNullOrWhiteSpace(value) && value.Length > maxLength)
            {
                return new ValidationError(fieldName, $"{fieldName} cannot exceed {maxLength} characters", value);
            }
            return null;
        }

        /// <summary>
        /// Validates numeric range
        /// </summary>
        /// <param name="value">Number to validate</param>
        /// <param name="minValue">Minimum allowed value</param>
        /// <param name="maxValue">Maximum allowed value</param>
        /// <param name="fieldName">Name of the field for error message</param>
        /// <returns>Validation error if invalid, null if valid</returns>
        public static ValidationError? ValidateRange(int? value, int minValue, int maxValue, string fieldName)
        {
            if (value.HasValue && (value.Value < minValue || value.Value > maxValue))
            {
                return new ValidationError(fieldName, $"{fieldName} must be between {minValue} and {maxValue}", value.Value);
            }
            return null;
        }
    }
}