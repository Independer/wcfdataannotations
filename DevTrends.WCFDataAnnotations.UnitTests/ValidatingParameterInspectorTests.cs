using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ServiceModel;
using Moq;
using NUnit.Framework;

namespace DevTrends.WCFDataAnnotations.UnitTests
{
    [TestFixture]
    public class ValidatingParameterInspectorTests
    {
        private const string OperationName = "DoSomething";

        private Mock<IObjectValidator> _singleValidatorMock;
        private Mock<IObjectValidator> _secondValidatorMock;
        private Mock<IErrorMessageGenerator> _errorMessageGeneratorMock;
        private ValidatingParameterInspector _singleValidatorParameterInspector;
        private ValidatingParameterInspector _multipleValidatorsParameterInspector;

        [SetUp]
        public void Setup()
        {
            _singleValidatorMock = new Mock<IObjectValidator>();
            _singleValidatorMock.Setup(x => x.Validate(It.IsAny<object>())).Returns(Enumerable.Empty<ValidationResult>());

            _secondValidatorMock = new Mock<IObjectValidator>();
            _secondValidatorMock.Setup(x => x.Validate(It.IsAny<object>())).Returns(Enumerable.Empty<ValidationResult>());

            _errorMessageGeneratorMock = new Mock<IErrorMessageGenerator>();
            _singleValidatorParameterInspector = new ValidatingParameterInspector(new[] { _singleValidatorMock.Object }, _errorMessageGeneratorMock.Object);
            _multipleValidatorsParameterInspector = new ValidatingParameterInspector(new[] { _singleValidatorMock.Object, _secondValidatorMock.Object }, _errorMessageGeneratorMock.Object);
        }        

        [Test]
        public void Must_Be_Supplied_With_Validators()
        {
            Assert.Throws<ArgumentNullException>(() => new ValidatingParameterInspector(null, null));
        }

        [Test]
        public void Validators_Cannot_Be_Empty()
        {
            Assert.Throws<ArgumentException>(() => new ValidatingParameterInspector(Enumerable.Empty<IObjectValidator>(), null));
        }

        [Test]
        public void Must_Be_Supplied_With_ErrorMessageGenerator()
        {
            Assert.Throws<ArgumentNullException>(() => new ValidatingParameterInspector(new[] { new NullCheckObjectValidator() }, null));
        }

        [Test]
        public void BeforeCall_Calls_Validator_For_Single_Input()
        {
            var input = new object();
            _singleValidatorParameterInspector.BeforeCall(OperationName, new[] { input });
            _singleValidatorMock.Verify(x => x.Validate(input), Times.Once());
        }

        [Test]
        public void BeforeCall_Calls_Validator_For_Multiple_Inputs()
        {
            const string input1 = "hello";
            const string input2 = "goodbye";
            _singleValidatorParameterInspector.BeforeCall(OperationName, new[] { input1, input2 });
            _singleValidatorMock.Verify(x => x.Validate(input1), Times.Once());
            _singleValidatorMock.Verify(x => x.Validate(input2), Times.Once());
        }

        [Test]
        public void BeforeCall_Calls_Multiple_Validators_For_Single_Input()
        {
            var input = new object();
            _multipleValidatorsParameterInspector.BeforeCall(OperationName, new[] { input });
            _singleValidatorMock.Verify(x => x.Validate(input), Times.Once());
            _secondValidatorMock.Verify(x => x.Validate(input), Times.Once());
        }

        [Test]
        public void BeforeCall_Calls_Multiple_Validators_For_Multiple_Inputs()
        {
            const string input1 = "hello";
            const string input2 = "goodbye";
            _multipleValidatorsParameterInspector.BeforeCall(OperationName, new[] { input1, input2 });
            _singleValidatorMock.Verify(x => x.Validate(input1), Times.Once());
            _singleValidatorMock.Verify(x => x.Validate(input2), Times.Once());
            _secondValidatorMock.Verify(x => x.Validate(input1), Times.Once());
            _secondValidatorMock.Verify(x => x.Validate(input2), Times.Once());
        }

        [Test]
        public void BeforeCall_Calls_ErrorMessageGenerator_When_Validator_Returns_ValidationResult()
        {
            var validationResults = new List<ValidationResult> { new ValidationResult("something bad") };
            _singleValidatorMock.Setup(x => x.Validate(It.IsAny<object>())).Returns(validationResults);

            try
            {
                _singleValidatorParameterInspector.BeforeCall(OperationName, new[] { new object() });
            }
            catch
            {                
                // suppress
            }
            
            _errorMessageGeneratorMock.Verify(x => x.GenerateErrorMessage(OperationName, validationResults), Times.Once());
        }

        [Test]
        public void BeforeCall_Throw_Exception_Using_ErrorMessageGenerator_ReturnValue_When_Validator_Returns_ValidationResult()
        {
            const string errorMessage = "something really bad";

            var validationResults = new List<ValidationResult> { new ValidationResult("something bad") };
            _singleValidatorMock.Setup(x => x.Validate(It.IsAny<object>())).Returns(validationResults);

            _errorMessageGeneratorMock.Setup(x => x.GenerateErrorMessage(OperationName, validationResults)).Returns(errorMessage);

            var faultException = Assert.Throws<FaultException>(
                () => _singleValidatorParameterInspector.BeforeCall(OperationName, new[] {new object()}));

            Assert.That(faultException.Message, Is.EqualTo(errorMessage));
        }

        [Test]
        public void BeforeCall_Returns_Null_When_Valid()
        {
            var result = _singleValidatorParameterInspector.BeforeCall(OperationName, new[] {new object()});

            Assert.That(result, Is.Null);
        }
    }
}
