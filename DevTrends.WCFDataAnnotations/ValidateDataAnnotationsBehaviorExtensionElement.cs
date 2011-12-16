using System;
using System.ServiceModel.Configuration;

namespace DevTrends.WCFDataAnnotations
{
    public class ValidateDataAnnotationsBehaviorExtensionElement : BehaviorExtensionElement
    {
        public override Type BehaviorType
        {
            get { return typeof(ValidateDataAnnotationsBehavior); }
        }

        protected override object CreateBehavior()
        {
            return new ValidateDataAnnotationsBehavior();
        }
    }
}
