using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DevTrends.WCFDataAnnotations {
  public interface IValidationResultsLogger {
    void LogValidationResults(string operationName, IEnumerable<ValidationResult> validationResults);
  }
}
