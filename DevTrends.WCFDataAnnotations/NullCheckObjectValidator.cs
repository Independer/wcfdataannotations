using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DevTrends.WCFDataAnnotations
{
    public class NullCheckObjectValidator : IObjectValidator
    {
        public IEnumerable<ValidationResult> Validate(object input)
        {
            if (input == null)
            {
                yield return new ValidationResult("Input is null.");                
            }            
        }
    }
}
