using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Independer.WCFDataAnnotations {
  /// <summary>
  /// Validates inputs using Data Annotations
  /// </summary>
  [AttributeUsage(AttributeTargets.Class)]
  public class ValidateDataAnnotationsBehavior : Attribute, IServiceBehavior {
    /// <summary>
    ///     The list of _validators
    /// </summary>
    private readonly List<IObjectValidator> _validators;
    
    /// <summary>
    ///     [Optional] Logger implementation for validation results
    /// </summary>
    private readonly IValidationResultsLogger _validationResultsLogger;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidateDataAnnotationsBehavior" /> class.
    /// </summary>
    /// <param name="validationResultsLogger">Optional validation results logger. It has to implement the interface <see cref="IValidationResultsLogger"/></param>
    public ValidateDataAnnotationsBehavior(Type validationResultsLogger = null) {
      if (validationResultsLogger != null) {
        if (!typeof(IValidationResultsLogger).IsAssignableFrom(validationResultsLogger)) {
          throw new ArgumentException($"The type of {validationResultsLogger} doesn't implement the interface '{typeof(IValidationResultsLogger)}'!");
        }

        _validationResultsLogger = (IValidationResultsLogger)Activator.CreateInstance(validationResultsLogger);
      }

      _validators = new List<IObjectValidator>
      {
        new NullCheckObjectValidator(),
        new DataAnnotationsObjectValidator(),
        new ValidatableObjectValidator()
      };
    }

    /// <summary>
    ///     Provides the ability to pass custom data to binding elements to support the contract implementation.
    /// </summary>
    /// <param name="serviceDescription">The service description of the service.</param>
    /// <param name="serviceHostBase">The host of the service.</param>
    /// <param name="endpoints">The service endpoints.</param>
    /// <param name="bindingParameters">Custom objects to which binding elements have access.</param>
    public void AddBindingParameters(
            ServiceDescription serviceDescription,
            ServiceHostBase serviceHostBase,
            Collection<ServiceEndpoint> endpoints,
            BindingParameterCollection bindingParameters) { }

    /// <summary>
    ///     Provides the ability to change run-time property values or insert custom extension objects such as error handlers,
    ///     message or parameter interceptors, security extensions, and other custom extension objects.
    /// </summary>
    /// <param name="serviceDescription">The service description.</param>
    /// <param name="serviceHostBase">The host that is currently being built.</param>
    public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase) {
      var operations =
        from dispatcher in serviceHostBase.ChannelDispatchers.Cast<ChannelDispatcher>()
        from endpoint in dispatcher.Endpoints
        from operation in endpoint.DispatchRuntime.Operations
        select operation;

      var contractOperations = serviceHostBase.Description.Endpoints.SelectMany(x => x.Contract.Operations).ToList();

      operations = operations.Where(op => contractOperations.Any(co => co.Name == op.Name));
	  
      var errorMessageGenerator = new ErrorMessageGenerator();

      foreach (var operation in operations) {
        var parameterInfo = GetParameterInfo(operation.Name, contractOperations);

        operation.ParameterInspectors.Add(new ValidatingParameterInspector(_validators, errorMessageGenerator, _validationResultsLogger, parameterInfo));
      }
    }

    /// <summary>
    ///     Provides the ability to inspect the service host and the service description to confirm that the service can run
    ///     successfully.
    /// </summary>
    /// <param name="serviceDescription">The service description.</param>
    /// <param name="serviceHostBase">The service host that is currently being constructed.</param>
    public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase) { }

    /// <summary>
    /// Gets the parameters information for the given operation
    /// </summary>
    /// <param name="operationName"></param>
    /// <param name="contractOperations"></param>
    /// <returns></returns>
    private ParameterDetailsInfo GetParameterInfo(string operationName, IEnumerable<OperationDescription> contractOperations) {
      var parameterInfo = new ParameterDetailsInfo();

      var parameters = GetParameters(contractOperations.Single(x => x.Name == operationName));

      foreach (var parameter in parameters.OrderBy(x => x.Position)) {
        var skipNullCheck = false;

        foreach (var customAttribute in parameter.GetCustomAttributes(inherit: false)) {
          if (customAttribute is AllowNullAttribute) {
            skipNullCheck = true;
          }
        }

        parameterInfo.ParameterDetails.Add(new ParameterDetails { Name = parameter.Name, Position = parameter.Position, SkipNullcheck = skipNullCheck });
      }

      return parameterInfo;
    }

    private IEnumerable<ParameterInfo> GetParameters(OperationDescription operationDescription) {
      var method = operationDescription.SyncMethod ?? operationDescription.TaskMethod;

      if (method == null) {
        throw new InvalidOperationException("Either SyncMethod or TaskMethod should have a value!");
      }

      return method.GetParameters();
    }
  }
}