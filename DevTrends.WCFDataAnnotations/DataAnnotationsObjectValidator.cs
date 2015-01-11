using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DevTrends.WCFDataAnnotations
{
    /// <summary>
    ///     Validates objects that have Data Annotation <see cref="ValidationAttribute" />
    ///     derived types applied.
    /// </summary>
    public class DataAnnotationsObjectValidator : IObjectValidator
    {
        /// <summary>
        ///     Validates the specified object.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(object value)
        {
            if (value == null)
            {
                yield break;
            }

            foreach (var validationResult in GetValidationResults(value.GetType(), value))
            {
                yield return validationResult;
            }
        }

        /// <summary>
        ///     Validates the properties.
        /// </summary>
        /// <param name="propertyDescriptor">The property descriptor.</param>
        /// <param name="container">The container.</param>
        /// <returns></returns>
        private IEnumerable<ValidationResult> ValidateProperties(PropertyDescriptor propertyDescriptor, object container)
        {
            var value = propertyDescriptor.GetValue(container);

            var context = new ValidationContext(container, null, null)
            {
                DisplayName = propertyDescriptor.DisplayName,
                MemberName = propertyDescriptor.Name
            };

            foreach (var validationAttibute in propertyDescriptor.Attributes.OfType<ValidationAttribute>())
            {
                var result = validationAttibute.GetValidationResult(value, context);

                if (result != ValidationResult.Success)
                {
                    yield return result;
                }
            }

            if (value != null)
            {
                foreach (var validationResult in GetValidationResults(propertyDescriptor.PropertyType, value))
                {
                    yield return validationResult;
                }
            }
        }

        /// <summary>
        ///     Gets the validation results.
        /// </summary>
        /// <param name="propertyType">Type of the property.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private IEnumerable<ValidationResult> GetValidationResults(Type propertyType, object value)
        {
            var enumerable = value as IEnumerable;

            if (enumerable != null)
            {
                foreach (var item in enumerable)
                {
                    foreach (var result in Validate(item))
                    {
                        yield return result;
                    }
                }
            }

            var properties = TypeDescriptor.GetProperties(propertyType)
                .Cast<PropertyDescriptor>()
                .Where(p => !p.IsReadOnly);

            foreach (var property in properties)
            {
                foreach (var result in ValidateProperties(property, value))
                {
                    yield return result;
                }
            }
        }
    }
}