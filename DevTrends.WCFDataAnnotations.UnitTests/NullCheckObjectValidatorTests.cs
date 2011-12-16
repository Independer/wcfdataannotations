using System.Linq;
using NUnit.Framework;

namespace DevTrends.WCFDataAnnotations.UnitTests
{
    [TestFixture]
    public class NullCheckObjectValidatorTests
    {
        private NullCheckObjectValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new NullCheckObjectValidator();
        }        

        [Test]
        public void Validate_Returns_ValidationResult_When_Passed_Null()
        {
            var result = _validator.Validate(null);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(1));
        }

        [Test]
        public void Validate_Does_Not_Return_ValidationResult_When_Not_Passed_Null()
        {
            var result = _validator.Validate("not null");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Any(), Is.False);            
        }
    }
}
