using System;
using System.ServiceModel.Configuration;

namespace DevTrends.WCFDataAnnotations
{
    /// <summary>
    /// Extension for creating a <see cref="ValidateDataAnnotationsBehavior"/> through
    /// configuration
    /// </summary>
    public class ValidateDataAnnotationsBehaviorExtensionElement : BehaviorExtensionElement
    {
        /// <summary>
        /// Gets the type of behavior.
        /// </summary>
        public override Type BehaviorType
        {
            get { return typeof(ValidateDataAnnotationsBehavior); }
        }

        /// <summary>
        /// Creates a behavior extension based on the current configuration settings.
        /// </summary>
        /// <returns>
        /// The behavior extension.
        /// </returns>
        protected override object CreateBehavior()
        {
            return new ValidateDataAnnotationsBehavior();
        }
    }
}
