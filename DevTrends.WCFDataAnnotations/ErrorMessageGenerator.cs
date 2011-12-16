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
            ValidateArguments(operationName, validationResults);

            var errorMessageBuilder = new StringBuilder();

            errorMessageBuilder.AppendFormat("Service operation {0} failed due to validation errors: \n\n", operationName);

            foreach (var validationResult in validationResults)
            {
                var memberName = validationResult.MemberNames.FirstOrDefault();

                if (memberName == null)
                {
                    errorMessageBuilder.AppendFormat("{0} \n", validationResult.ErrorMessage);
                }
                else
                {
                    errorMessageBuilder.AppendFormat("{0}: {1} \n", memberName, validationResult.ErrorMessage);
                }
            }

            return errorMessageBuilder.ToString();
        }

        private static void ValidateArguments(string operationName, IEnumerable<ValidationResult> validationResults)
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
                throw new ArgumentException("At least one validationResult is required");
            }
        }
    }
}
