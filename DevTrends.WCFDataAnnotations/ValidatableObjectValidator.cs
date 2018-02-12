using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DevTrends.WCFDataAnnotations {
  /// <summary>
  /// Validates objects that implement <see cref="IValidatableObject"/>
  /// </summary>
  public class ValidatableObjectValidator : IObjectValidator {
    /// <summary>
    /// Validates the object.
    /// </summary>
    /// <param name="value">The object to validate.</param>
    /// <returns>
    /// A collection of <see cref="ValidationResult" /> containing information
    /// about validation errors
    /// </returns>
    public IEnumerable<ValidationResult> Validate(object value) {
      if (value == null) {
        yield break;
      }

      if (value is IValidatableObject validatableInput) {
        var context = new ValidationContext(value, null, null);

        foreach (var result in validatableInput.Validate(context)) {
          yield return result;
        }
      }

      if (value is IEnumerable enumerable) {
        var validationResults = ValidateEnumerable(enumerable);
        foreach (var validationResult in validationResults) {
          yield return validationResult;
        }
      }

      var properties = TypeDescriptor.GetProperties(value.GetType())
        .Cast<PropertyDescriptor>()
        .Where(p => !p.IsReadOnly);

      foreach (var property in properties) {
        foreach (var result in Validate(property.GetValue(value))) {
          yield return result;
        }
      }
    }

    /// <summary>
    /// Rescursively validate enumerables
    /// </summary>
    /// <param name="enumerable"></param>
    /// <returns>If validation fails, it returns the validation results</returns>
    private IEnumerable<ValidationResult> ValidateEnumerable(IEnumerable enumerable) {
      foreach (var item in enumerable) {
        if (item is IEnumerable nestedEnumerable) {
          var nestedValidationResults = ValidateEnumerable(nestedEnumerable);
          foreach (var nestedValidationResult in nestedValidationResults) {
            yield return nestedValidationResult;
          }
        }

        foreach (var result in Validate(item)) {
          yield return result;
        }
      }
    }
  }
}