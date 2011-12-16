using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DevTrends.WCFDataAnnotations
{
    public interface IErrorMessageGenerator
    {
        string GenerateErrorMessage(string operationName, IEnumerable<ValidationResult> validationResults);
    }
}