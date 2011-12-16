using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DevTrends.WCFDataAnnotations
{
    public interface IObjectValidator
    {
        IEnumerable<ValidationResult> Validate(object input);
    }
}