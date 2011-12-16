using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace DevTrends.WCFDataAnnotations
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ValidateDataAnnotationsBehavior : Attribute, IServiceBehavior
    {
        private readonly IParameterInspector _validatingParameterInspector;

        public ValidateDataAnnotationsBehavior()
        {
            var validators = new IObjectValidator[]
                                 {
                                     new NullCheckObjectValidator(),
                                     new DataAnnotationsObjectValidator(),
                                     new ValidatableObjectValidator()
                                 };

            _validatingParameterInspector = new ValidatingParameterInspector(validators, new ErrorMessageGenerator());
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase,
                                         Collection<ServiceEndpoint> endpoints,
                                         BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            var operations =
                from dispatcher in serviceHostBase.ChannelDispatchers.Cast<ChannelDispatcher>()
                from endpoint in dispatcher.Endpoints
                from operation in endpoint.DispatchRuntime.Operations
                select operation;

            operations.ToList().ForEach(operation => operation.ParameterInspectors.Add(_validatingParameterInspector));
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }
    }
}