using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DevTrends.WCFDataAnnotations
{
    public class DataAnnotationsObjectValidator : IObjectValidator
    {
        public IEnumerable<ValidationResult> Validate(object input)
        {
            if (input == null) return Enumerable.Empty<ValidationResult>();

            return from property in TypeDescriptor.GetProperties(input).Cast<PropertyDescriptor>()
                   from attribute in property.Attributes.OfType<ValidationAttribute>()
                   where !attribute.IsValid(property.GetValue(input))
                   select new ValidationResult
                   (
                       attribute.FormatErrorMessage(string.Empty),
                       new[] { property.Name }
                   );
        }
    }
}
