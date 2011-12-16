using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NUnit.Framework;

namespace DevTrends.WCFDataAnnotations.UnitTests
{
    [TestFixture]
    public class ValidatableObjectValidatorTests
    {
        private const string ErrorMessage = "oops message";

        private ValidatableObjectValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new ValidatableObjectValidator();
        }        

        [Test]
        public void Validate_Does_Not_Return_ValidationResult_When_Passed_Null()
        {
            var result = _validator.Validate(null);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Any(), Is.False);            
        }

        [Test]
        public void Validate_Does_Not_Return_ValidationResult_When_Not_Passed_IValidatableObject()
        {
            var result = _validator.Validate("blah");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Any(), Is.False);
        }

        [Test]
        public void Validate_Does_Not_Return_ValidationResult_When_Passed_IValidatableObject_That_Is_Valid()
        {
            var result = _validator.Validate(new ValidValidatableObject());

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Any(), Is.False);
        }

        [Test]
        public void Validate_Returns_ValidationResult_When_Passed_IValidatableObject_That_Is_Not_Valid()
        {
            var result = _validator.Validate(new InvalidValidatableObject());

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().ErrorMessage, Is.StringContaining(ErrorMessage));
        }

        private class ValidValidatableObject : IValidatableObject
        {
            public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
            {
                return Enumerable.Empty<ValidationResult>();
            }
        }

        private class InvalidValidatableObject : IValidatableObject
        {
            public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
            {
                yield return new ValidationResult(ErrorMessage);
            }
        }
    }
}
