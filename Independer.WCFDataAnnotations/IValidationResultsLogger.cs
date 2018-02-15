using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Independer.WCFDataAnnotations {
  public interface IValidationResultsLogger {
    void LogValidationResults(string operationName, IEnumerable<ValidationResult> validationResults);
  }
}
