using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;

namespace Independer.WCFDataAnnotations {
  /// <summary>
  /// Validates incoming parameters using Data Annotations before executing
  /// the service operation call
  /// </summary>
  public class ValidatingParameterInspector : IParameterInspector {
    private readonly IEnumerable<IObjectValidator> _validators;
    private readonly IErrorMessageGenerator _errorMessageGenerator;
    private readonly ParameterDetailsInfo _parameterDetailsInfo;
    private readonly IValidationResultsLogger _validationResultsLogger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidatingParameterInspector"/> class.
    /// </summary>
    /// <param name="validators">The validators.</param>
    /// <param name="errorMessageGenerator">The error message generator.</param>
    /// <param name="validationResultsLogger">[Optional] The validation results logger</param>
    /// <param name="parameterDetailsInfo">The parameter info object for the optional validator skipping.</param>
    public ValidatingParameterInspector(IEnumerable<IObjectValidator> validators, IErrorMessageGenerator errorMessageGenerator, IValidationResultsLogger validationResultsLogger = null, ParameterDetailsInfo parameterDetailsInfo = null) {
      _validationResultsLogger = validationResultsLogger;

      if (validators == null) {
        throw new ArgumentNullException(nameof(validators));
      }

      if (!validators.Any()) {
        throw new ArgumentException("At least one validator is required.");
      }

      if (errorMessageGenerator == null) {
        throw new ArgumentNullException(nameof(errorMessageGenerator));
      }

      _validators = validators;
      _errorMessageGenerator = errorMessageGenerator;
      _parameterDetailsInfo = parameterDetailsInfo;
    }

    /// <summary>
    /// Called after client calls are returned and before service responses are sent.
    /// </summary>
    /// <param name="operationName">The name of the invoked operation.</param>
    /// <param name="outputs">Any output objects.</param>
    /// <param name="returnValue">The return value of the operation.</param>
    /// <param name="correlationState">Any correlation state returned from the 
    /// <see cref="M:System.ServiceModel.Dispatcher.IParameterInspector.BeforeCall(System.String,System.Object[])" /> method, 
    /// or null.</param>
    public void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState) { }

    /// <summary>
    /// Called before client calls are sent and after service responses are returned.
    /// </summary>
    /// <param name="operationName">The name of the operation.</param>
    /// <param name="inputs">The objects passed to the method by the client.</param>
    /// <returns>
    /// The correlation state that is returned as the <paramref name="correlationState" /> 
    /// parameter in <see cref="M:System.ServiceModel.Dispatcher.IParameterInspector.AfterCall(System.String,System.Object[],System.Object,System.Object)" />. 
    /// Return null if you do not intend to use correlation state.
    /// </returns>
    /// <exception cref="System.ServiceModel.FaultException"></exception>
    public object BeforeCall(string operationName, object[] inputs) {
      var parameterPosition = 0;
      var validationResults = new List<ValidationResult>();

      foreach (var input in inputs) {
        var parameterDetails = GetParameterDetails(parameterPosition);

        foreach (var validator in GetValidators(parameterDetails)) {
          var results = validator.Validate(input);
          
          validationResults.AddRange(AddParameterNamesWhenMissing(results, parameterDetails?.Name));
        }
        parameterPosition++;
      }

      if (validationResults.Count > 0) {
        _validationResultsLogger?.LogValidationResults(operationName, validationResults);
        throw new FaultException(_errorMessageGenerator.GenerateErrorMessage(operationName, validationResults));
      }

      return null;
    }

    private IEnumerable<ValidationResult> AddParameterNamesWhenMissing(IEnumerable<ValidationResult> validationResults, string parameterName) {
      if (validationResults == null || !validationResults.Any() || String.IsNullOrWhiteSpace(parameterName)) {
        return validationResults;
      }

      var extendedValidationResults = new List<ValidationResult>();

      foreach (var validationResult in validationResults) {
        extendedValidationResults.Add(validationResult.MemberNames == null || !validationResult.MemberNames.Any() 
          ? new ValidationResult(validationResult.ErrorMessage, new List<string> { parameterName }) 
          : validationResult);
      }

      return extendedValidationResults;
    }

    private ParameterDetails GetParameterDetails(int parameterPosition) {
      return _parameterDetailsInfo?.ParameterDetails.Single(x => x.Position == parameterPosition);
    }

    private IEnumerable<IObjectValidator> GetValidators(ParameterDetails parameterDetails) {
      if (parameterDetails == null) {
        throw new ArgumentNullException(nameof(parameterDetails));
      }

      return parameterDetails.SkipNullcheck
        ? _validators.Where(x => !(x is NullCheckObjectValidator))
        : _validators;
    }
  }
}
