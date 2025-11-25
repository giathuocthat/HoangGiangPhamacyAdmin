using System.ComponentModel.DataAnnotations;

namespace ThuocGiaThatAdmin.Server.Attributes
{
    /// <summary>
    /// Validates that a value is greater than zero
    /// </summary>
    public class GreaterThanZeroAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            if (value is int intValue && intValue > 0)
                return ValidationResult.Success;

            if (value is long longValue && longValue > 0)
                return ValidationResult.Success;

            if (value is decimal decimalValue && decimalValue > 0)
                return ValidationResult.Success;

            if (value is double doubleValue && doubleValue > 0)
                return ValidationResult.Success;

            return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} must be greater than zero");
        }
    }

    /// <summary>
    /// Validates that a value is not in the future
    /// </summary>
    public class NotFutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            if (value is System.DateTime dateValue && dateValue <= System.DateTime.UtcNow)
                return ValidationResult.Success;

            return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} cannot be in the future");
        }
    }

    /// <summary>
    /// Validates that a string contains only alphanumeric characters and specific allowed characters
    /// </summary>
    public class AlphanumericWithSpecialCharsAttribute : ValidationAttribute
    {
        private readonly string _allowedChars;

        public AlphanumericWithSpecialCharsAttribute(string allowedChars = "-_")
        {
            _allowedChars = allowedChars;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            var stringValue = value.ToString();
            
            foreach (char c in stringValue)
            {
                if (!char.IsLetterOrDigit(c) && !_allowedChars.Contains(c))
                {
                    return new ValidationResult(
                        ErrorMessage ?? $"{validationContext.DisplayName} can only contain letters, numbers, and the following characters: {_allowedChars}");
                }
            }

            return ValidationResult.Success;
        }
    }

    /// <summary>
    /// Validates that a collection has at least one item
    /// </summary>
    public class RequiredCollectionAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} is required");

            if (value is System.Collections.IEnumerable enumerable)
            {
                var enumerator = enumerable.GetEnumerator();
                if (enumerator.MoveNext())
                    return ValidationResult.Success;
            }

            return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} must contain at least one item");
        }
    }

    /// <summary>
    /// Validates that a value is within a specific range
    /// </summary>
    public class PageSizeAttribute : RangeAttribute
    {
        public PageSizeAttribute() : base(1, 100)
        {
            ErrorMessage = "Page size must be between {1} and {2}";
        }
    }

    /// <summary>
    /// Validates that a value is a valid page number (greater than 0)
    /// </summary>
    public class PageNumberAttribute : RangeAttribute
    {
        public PageNumberAttribute() : base(1, int.MaxValue)
        {
            ErrorMessage = "Page number must be greater than 0";
        }
    }
}
