using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NUnit.Framework;

namespace DevTrends.WCFDataAnnotations.UnitTests {
  [TestFixture]
  [Category(nameof(ValidatableObjectValidatorTests))]
  public class ValidatableObjectValidatorTests {
    private const string ErrorMessage = "oops message";
    private ValidatableObjectValidator _validator;

    [SetUp]
    public void Setup() {
      _validator = new ValidatableObjectValidator();
    }

    [Test]
    public void Validate_Does_Not_Return_ValidationResult_When_Passed_Null() {
      var result = _validator.Validate(null);

      Assert.That(result, Is.Not.Null);
      Assert.That(result.Any(), Is.False);
    }

    [Test]
    public void Validate_Does_Not_Return_ValidationResult_When_Not_Passed_IValidatableObject() {
      var result = _validator.Validate("blah");

      Assert.That(result, Is.Not.Null);
      Assert.That(result.Any(), Is.False);
    }

    [Test]
    public void Validate_Does_Not_Return_ValidationResult_When_Passed_IValidatableObject_That_Is_Valid() {
      var result = _validator.Validate(new ValidValidatableObject());

      Assert.That(result, Is.Not.Null);
      Assert.That(result.Any(), Is.False);
    }

    [Test]
    public void Validate_Returns_ValidationResult_When_Passed_IValidatableObject_That_Is_Not_Valid() {
      var result = _validator.Validate(new InvalidValidatableObject());

      Assert.That(result, Is.Not.Null);
      Assert.That(result.Count(), Is.EqualTo(1));
      Assert.That(result.First().ErrorMessage, Is.StringContaining(ErrorMessage));
    }

    [Test]
    public void Validate_Returns_ValidationResult_When_Passed_Nested_IValidatableObject_That_Is_Not_Valid() {
      var result = _validator.Validate(new ValidatableObjectNested { InvalidValidatableObject = new InvalidValidatableObject() });

      Assert.That(result, Is.Not.Null);
      Assert.That(result.Count(), Is.EqualTo(1));
      Assert.That(result.First().ErrorMessage, Is.StringContaining(ErrorMessage));
    }

    [Test]
    public void Validate_Returns_ValidationResult_When_Passed_Enumerable_IValidatableObject_That_Is_Not_Valid() {
      var result = _validator.Validate(new ValidatableObjectEnumerable { InvalidValidatableObjectList = new List<InvalidValidatableObject> { new InvalidValidatableObject(), new InvalidValidatableObject() }});

      Assert.That(result, Is.Not.Null);
      Assert.That(result.Count(), Is.EqualTo(2));
      Assert.That(result.First().ErrorMessage, Is.StringContaining(ErrorMessage));
    }

    [Test]
    public void Validate_Returns_ValidationResult_When_Passed_Nested_Enumerable_IValidatableObject_That_Is_Not_Valid() {
      var result = _validator.Validate(new ValidatableObjectNestedEnumerable {
        ValidatableObjectSubEnumerableList = new List<ValidatableObjectNestedSubEnumerable> {
          new ValidatableObjectNestedSubEnumerable { InvalidValidatableObjectList = new List<InvalidValidatableObject> {
            new InvalidValidatableObject(),
            new InvalidValidatableObject()
          }}
        }
      });

      Assert.That(result, Is.Not.Null);
      Assert.That(result.Count(), Is.EqualTo(2));
      Assert.That(result.First().ErrorMessage, Is.StringContaining(ErrorMessage));
    }

    private class ValidValidatableObject : IValidatableObject {
      public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
        return Enumerable.Empty<ValidationResult>();
      }
    }

    private class InvalidValidatableObject : IValidatableObject {
      public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
        yield return new ValidationResult(ErrorMessage);
      }
    }

    private class ValidatableObjectNested : IValidatableObject {
      public InvalidValidatableObject InvalidValidatableObject { get; set; }

      public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
        return Enumerable.Empty<ValidationResult>();
      }
    }

    private class ValidatableObjectEnumerable : IValidatableObject {
      public List<InvalidValidatableObject> InvalidValidatableObjectList { get; set; }

      public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
        return Enumerable.Empty<ValidationResult>();
      }
    }

    private class ValidatableObjectNestedEnumerable : IValidatableObject {
      public List<ValidatableObjectNestedSubEnumerable> ValidatableObjectSubEnumerableList { get; set; }

      public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
        return Enumerable.Empty<ValidationResult>();
      }
    }

    private class ValidatableObjectNestedSubEnumerable : IValidatableObject {
      public List<InvalidValidatableObject> InvalidValidatableObjectList { get; set; }

      public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
        return Enumerable.Empty<ValidationResult>();
      }
    }
  }
}