using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Independer.WCFDataAnnotations {
  /// <summary>
  /// Default error message generation that uses the error messages from the 
  /// <see cref="IEnumerable{ValidationResult}"/>
  /// </summary>
  public class ErrorMessageGenerator : IErrorMessageGenerator {
    /// <summary>
    /// Generates the error message.
    /// </summary>
    /// <param name="operationName">Name of the operation.</param>
    /// <param name="validationResults">The validation results.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">
    /// operationName
    /// or
    /// validationResults
    /// </exception>
    /// <exception cref="System.ArgumentException">At least one ValidationResult is required</exception>
    public string GenerateErrorMessage(string operationName, IEnumerable<ValidationResult> validationResults) {
      if (operationName == null) {
        throw new ArgumentNullException(nameof(operationName));
      }

      if (validationResults == null) {
        throw new ArgumentNullException(nameof(validationResults));
      }

      if (!validationResults.Any()) {
        throw new ArgumentException("At least one ValidationResult is required");
      }

      var errorMessageBuilder = new StringBuilder();

      errorMessageBuilder.AppendFormat(
          "Service operation {0} failed due to validation errors: {1}{1}",
          operationName,
          Environment.NewLine);

      foreach (var validationResult in validationResults) {
        errorMessageBuilder.AppendFormat(
            "{0} {1}",
            validationResult.ErrorMessage,
            Environment.NewLine);
      }

      return errorMessageBuilder.ToString();
    }
  }
}
