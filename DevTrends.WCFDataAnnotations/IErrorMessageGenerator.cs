using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DevTrends.WCFDataAnnotations {
  /// <summary>
  /// Generates an error message for an operation based on the validation results
  /// </summary>
  public interface IErrorMessageGenerator {
    /// <summary>
    /// Generates the error message.
    /// </summary>
    /// <param name="operationName">Name of the operation.</param>
    /// <param name="validationResults">The validation results.</param>
    /// <returns></returns>
    string GenerateErrorMessage(string operationName, IEnumerable<ValidationResult> validationResults);
  }
}