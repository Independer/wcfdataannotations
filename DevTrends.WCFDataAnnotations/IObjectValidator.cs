using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DevTrends.WCFDataAnnotations
{
    /// <summary>
    /// Validates objects.
    /// </summary>
    public interface IObjectValidator
    {
        /// <summary>
        ///     Validates the object.
        /// </summary>
        /// <param name="value">The object to validate.</param>
        /// <returns>
        ///     A collection of <see cref="ValidationResult" /> containing information
        ///     about validation errors
        /// </returns>
        IEnumerable<ValidationResult> Validate(object value);
    }
}