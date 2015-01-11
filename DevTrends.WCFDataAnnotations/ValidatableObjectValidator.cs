using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DevTrends.WCFDataAnnotations
{
    /// <summary>
    /// Validates objects that implement <see cref="IValidatableObject"/>
    /// </summary>
    public class ValidatableObjectValidator : IObjectValidator
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
            var validatableInput = value as IValidatableObject;

            if (validatableInput != null)
            {
                var context = new ValidationContext(value, null, null);

                foreach (var result in validatableInput.Validate(context))
                {
                    yield return result;
                }
            }
        }
    }
}