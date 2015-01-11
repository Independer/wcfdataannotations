using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DevTrends.WCFDataAnnotations
{
    /// <summary>
    /// Validates that an object is not null
    /// </summary>
    public class NullCheckObjectValidator : IObjectValidator
    {
        /// <summary>
        /// Validates the object.
        /// </summary>
        /// <param name="value">The object to validate.</param>
        /// <returns>
        /// A collection of <see cref="ValidationResult" /> containing information
        /// about validation errors
        /// </returns>
        public IEnumerable<ValidationResult> Validate(object value)
        {
            if (value == null)
            {
                yield return new ValidationResult("Input is null.");                
            }            
        }
    }
}
