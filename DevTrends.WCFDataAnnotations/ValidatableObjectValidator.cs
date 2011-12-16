using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DevTrends.WCFDataAnnotations
{
    public class ValidatableObjectValidator : IObjectValidator
    {
        public IEnumerable<ValidationResult> Validate(object input)
        {
            var validatableInput = input as IValidatableObject;

            if (validatableInput != null)
            {
                return validatableInput.Validate(new ValidationContext(input, null, null));
            }

            return Enumerable.Empty<ValidationResult>();
        }
    }
}
