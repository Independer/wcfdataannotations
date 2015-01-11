using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace DevTrends.WCFDataAnnotations
{
    public class ErrorMessageGenerator : IErrorMessageGenerator
    {
        public string GenerateErrorMessage(string operationName, IEnumerable<ValidationResult> validationResults)
        {
            if (operationName == null)
            {
                throw new ArgumentNullException("operationName");
            }

            if (validationResults == null)
            {
                throw new ArgumentNullException("validationResults");
            }

            if (!validationResults.Any())
            {
                throw new ArgumentException("At least one ValidationResult is required");
            }

            var errorMessageBuilder = new StringBuilder();

            errorMessageBuilder.AppendFormat(
                "Service operation {0} failed due to validation errors: {1}{1}",
                operationName,
                Environment.NewLine);

            foreach (var validationResult in validationResults)
            {
                errorMessageBuilder.AppendFormat(
                    "{0} {1}",
                    validationResult.ErrorMessage,
                    Environment.NewLine);
            }

            return errorMessageBuilder.ToString();
        }
    }
}
