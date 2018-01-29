using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace DevTrends.WCFDataAnnotations {
  /// <summary>
  /// Validates inputs using Data Annotations
  /// </summary>
  [AttributeUsage(AttributeTargets.Class)]
  public class ValidateDataAnnotationsBehavior : Attribute, IServiceBehavior {
    /// <summary>
    ///     The default _validating parameter inspector
    /// </summary>
    private readonly IParameterInspector _defaultValidatingParameterInspector;

    /// <summary>
    ///     The list of _validators
    /// </summary>
    private readonly List<IObjectValidator> _validators;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidateDataAnnotationsBehavior" /> class.
    /// </summary>
    public ValidateDataAnnotationsBehavior() {
      _validators = new List<IObjectValidator>
      {
        new NullCheckObjectValidator(),
        new DataAnnotationsObjectValidator(),
        new ValidatableObjectValidator()
      };

      var errorMessageGenerator = new ErrorMessageGenerator();

      _defaultValidatingParameterInspector = new ValidatingParameterInspector(_validators, errorMessageGenerator);
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

      var contractMethods = serviceHostBase.Description.Endpoints.SelectMany(x => x.Contract.ContractType.GetMethods()).ToList();

      foreach (var operation in operations) {
        var parameterInfo = GetParameterInfo(operation.Name, contractMethods);

        operation.ParameterInspectors.Add(parameterInfo.HasAnyParameterSkipNullCheck
          ? new ValidatingParameterInspector(_validators, new ErrorMessageGenerator(), parameterInfo)
          : _defaultValidatingParameterInspector);
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
    /// <param name="contractMethods"></param>
    /// <returns></returns>
    private ParameterInfo GetParameterInfo(string operationName, IEnumerable<MethodInfo> contractMethods) {
      var parameterInfo = new ParameterInfo();

      var parameters = contractMethods.Single(x => x.Name == operationName).GetParameters();
      
      foreach (var parameter in parameters.OrderBy(x => x.Position)) {
        var skipNullCheck = false;

        foreach (var customAttribute in parameter.GetCustomAttributes(inherit: false)) {
          if (customAttribute is SkipNullCheckAttribute) {
            skipNullCheck = true;
          }
        }

        parameterInfo.ParameterDetails.Add(new ParameterDetails { Name = parameter.Name, Position = parameter.Position, SkipNullcheck = skipNullCheck });
      }

      return parameterInfo;
    }
  }
}