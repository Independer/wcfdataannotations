using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ServiceModel;
using Independer.WCFDataAnnotations.UnitTests.Helpers;
using Moq;
using NUnit.Framework;

namespace Independer.WCFDataAnnotations.UnitTests {
  [TestFixture]
  [Category(nameof(ValidatingParameterInspectorTests))]
  public class ValidatingParameterInspectorTests {
    private const string OperationName = "DoSomething";

    private Mock<IObjectValidator> _singleValidatorMock;
    private Mock<IObjectValidator> _secondValidatorMock;
    private Mock<IErrorMessageGenerator> _errorMessageGeneratorMock;
    private ValidatingParameterInspector _singleValidatorParameterInspector;
    private ValidatingParameterInspector _multipleValidatorsParameterInspector;

    private ParameterDetailsInfo _defaultParameterDetailsInfo;

    [SetUp]
    public void Setup() {
      _defaultParameterDetailsInfo = new ParameterDetailsInfo { ParameterDetails = new List<ParameterDetails> { new ParameterDetails { Name = "test", Position = 0, SkipNullcheck = false }, new ParameterDetails { Name = "test2", Position = 1, SkipNullcheck = false } } };

      _singleValidatorMock = new Mock<IObjectValidator>();
      _singleValidatorMock.Setup(x => x.Validate(It.IsAny<object>())).Returns(Enumerable.Empty<ValidationResult>());

      _secondValidatorMock = new Mock<IObjectValidator>();
      _secondValidatorMock.Setup(x => x.Validate(It.IsAny<object>())).Returns(Enumerable.Empty<ValidationResult>());

      _errorMessageGeneratorMock = new Mock<IErrorMessageGenerator>();
      _singleValidatorParameterInspector = new ValidatingParameterInspector(new[] { _singleValidatorMock.Object }, _errorMessageGeneratorMock.Object, null , _defaultParameterDetailsInfo);
      _multipleValidatorsParameterInspector = new ValidatingParameterInspector(new[] { _singleValidatorMock.Object, _secondValidatorMock.Object }, _errorMessageGeneratorMock.Object, null, _defaultParameterDetailsInfo);
    }

    [Test]
    public void Must_Be_Supplied_With_Validators() {
      Assert.Throws<ArgumentNullException>(() => new ValidatingParameterInspector(null, null));
    }

    [Test]
    public void Validators_Cannot_Be_Empty() {
      Assert.Throws<ArgumentException>(() => new ValidatingParameterInspector(Enumerable.Empty<IObjectValidator>(), null));
    }

    [Test]
    public void Must_Be_Supplied_With_ErrorMessageGenerator() {
      Assert.Throws<ArgumentNullException>(() => new ValidatingParameterInspector(new[] { new NullCheckObjectValidator() }, null));
    }

    [Test]
    public void BeforeCall_Calls_Validator_For_Single_Input() {
      var input = new object();
      _singleValidatorParameterInspector.BeforeCall(OperationName, new[] { input });
      _singleValidatorMock.Verify(x => x.Validate(input), Times.Once());
    }

    [Test]
    public void BeforeCall_Calls_Validator_For_Multiple_Inputs() {
      const string input1 = "hello";
      const string input2 = "goodbye";
      _singleValidatorParameterInspector.BeforeCall(OperationName, new[] { input1, input2 });
      _singleValidatorMock.Verify(x => x.Validate(input1), Times.Once());
      _singleValidatorMock.Verify(x => x.Validate(input2), Times.Once());
    }

    [Test]
    public void BeforeCall_Calls_Multiple_Validators_For_Single_Input() {
      var input = new object();
      _multipleValidatorsParameterInspector.BeforeCall(OperationName, new[] { input });
      _singleValidatorMock.Verify(x => x.Validate(input), Times.Once());
      _secondValidatorMock.Verify(x => x.Validate(input), Times.Once());
    }

    [Test]
    public void BeforeCall_Calls_Multiple_Validators_For_Multiple_Inputs() {
      const string input1 = "hello";
      const string input2 = "goodbye";
      _multipleValidatorsParameterInspector.BeforeCall(OperationName, new[] { input1, input2 });
      _singleValidatorMock.Verify(x => x.Validate(input1), Times.Once());
      _singleValidatorMock.Verify(x => x.Validate(input2), Times.Once());
      _secondValidatorMock.Verify(x => x.Validate(input1), Times.Once());
      _secondValidatorMock.Verify(x => x.Validate(input2), Times.Once());
    }

    [Test]
    public void BeforeCall_Calls_ErrorMessageGenerator_When_Validator_Returns_ValidationResult() {
      var validationResults = new List<ValidationResult> { new ValidationResult("something bad", new[] { "test" }) };
      _singleValidatorMock.Setup(x => x.Validate(It.IsAny<object>())).Returns(validationResults);

      try {
        _singleValidatorParameterInspector.BeforeCall(OperationName, new[] { new object() });
      }
      catch {
        // suppress
      }

      _errorMessageGeneratorMock.Verify(x => x.GenerateErrorMessage(OperationName, MoqParameter.ShouldBeEquivalentTo(validationResults)), Times.Once());
    }

    [Test]
    public void BeforeCall_Throw_Exception_Using_ErrorMessageGenerator_ReturnValue_When_Validator_Returns_ValidationResult() {
      const string errorMessage = "something really bad";

      var validationResults = new List<ValidationResult> { new ValidationResult("something bad", new []{ "test" }) };
      _singleValidatorMock.Setup(x => x.Validate(It.IsAny<object>())).Returns(validationResults);

      _errorMessageGeneratorMock.Setup(x => x.GenerateErrorMessage(OperationName, validationResults)).Returns(errorMessage);

      var faultException = Assert.Throws<FaultException>(
          () => _singleValidatorParameterInspector.BeforeCall(OperationName, new[] { new object() }));

      Assert.That(faultException.Message, Is.EqualTo(errorMessage));
    }

    [Test]
    public void BeforeCall_Call_Logger_And_Throw_Exception_Using_ErrorMessageGenerator_ReturnValue_When_Validator_Returns_ValidationResult() {
      const string errorMessage = "something really bad";

      var validationResults = new List<ValidationResult> { new ValidationResult("something bad", new[] { "test" }) };
      _singleValidatorMock.Setup(x => x.Validate(It.IsAny<object>())).Returns(validationResults);

      _errorMessageGeneratorMock.Setup(x => x.GenerateErrorMessage(OperationName, validationResults)).Returns(errorMessage);

      var loggerMock = new Mock<IValidationResultsLogger>();
      loggerMock.Setup(x => x.LogValidationResults(OperationName,MoqParameter.ShouldBeEquivalentTo(validationResults)));

      var inspector = new ValidatingParameterInspector(new[] { _singleValidatorMock.Object, _secondValidatorMock.Object, new NullCheckObjectValidator() }, _errorMessageGeneratorMock.Object, loggerMock.Object, _defaultParameterDetailsInfo);

      Assert.Throws<FaultException>(() => inspector.BeforeCall(OperationName, new[] { new object() }));
      loggerMock.Verify(x => x.LogValidationResults(OperationName, MoqParameter.ShouldBeEquivalentTo(validationResults)), Times.Once());
    }

    [Test]
    public void BeforeCall_Returns_Null_When_Valid() {
      var result = _singleValidatorParameterInspector.BeforeCall(OperationName, new[] { new object() });

      Assert.That(result, Is.Null);
    }

    [Test]
    public void BeforeCall_SkipNullCheck() {
      var parameterInfo = new ParameterDetailsInfo { ParameterDetails = new List<ParameterDetails> { new ParameterDetails { Name = "test", Position = 0, SkipNullcheck = true } } };
      var inspector = new ValidatingParameterInspector(new[] { _singleValidatorMock.Object, _secondValidatorMock.Object, new NullCheckObjectValidator() }, _errorMessageGeneratorMock.Object, null, parameterInfo);
      object input = null;
      inspector.BeforeCall(OperationName, new[] { input });
      _singleValidatorMock.Verify(x => x.Validate(input), Times.Once());
    }

    [Test]
    public void BeforeCall_SkipNullCheck_With_Multiple_Parameters() {
      var parameterInfo = new ParameterDetailsInfo {
        ParameterDetails = new List<ParameterDetails> {
          new ParameterDetails { Name = "test1", Position = 0, SkipNullcheck = false },
          new ParameterDetails { Name = "test2", Position = 1, SkipNullcheck = true },
          new ParameterDetails { Name = "test3", Position = 2, SkipNullcheck = false }
        }
      };
      var inspector = new ValidatingParameterInspector(new[] { _singleValidatorMock.Object, _secondValidatorMock.Object, new NullCheckObjectValidator() }, _errorMessageGeneratorMock.Object, null, parameterInfo);
      object input1 = new object();
      object input2 = null;
      object input3 = "test";
      inspector.BeforeCall(OperationName, new[] { input1, input2, input3 });
    }

    [Test]
    public void BeforeCall_SkipNullCheck_With_Multiple_Parameters_Should_Throw_If_Not_SkipNullCheck_Marked_Parameter_Is_Null() {
      var parameterInfo = new ParameterDetailsInfo {
        ParameterDetails = new List<ParameterDetails> {
          new ParameterDetails { Name = "test1", Position = 0, SkipNullcheck = false },
          new ParameterDetails { Name = "test2", Position = 1, SkipNullcheck = true },
          new ParameterDetails { Name = "test3", Position = 2, SkipNullcheck = false }
        }
      };
      var inspector = new ValidatingParameterInspector(new[] { _singleValidatorMock.Object, _secondValidatorMock.Object, new NullCheckObjectValidator() }, _errorMessageGeneratorMock.Object, null, parameterInfo);
      object input1 = new object();
      object input2 = "test";
      object input3 = null;

      Assert.Throws<FaultException>(
        () => inspector.BeforeCall(OperationName, new[] { input1, input2, input3 }));
    }

    [Test]
    public void BeforeCall_Throws_Exception_On_Null_When_No_SkipNullCheck_Defined() {
      var inspector = new ValidatingParameterInspector(new[] { _singleValidatorMock.Object, _secondValidatorMock.Object, new NullCheckObjectValidator() }, _errorMessageGeneratorMock.Object, null, _defaultParameterDetailsInfo);
      object input = null;

      Assert.Throws<FaultException>(
        () => inspector.BeforeCall(OperationName, new[] { input }));
    }

    [Test]
    public void BeforeCall_Throws_Exception_On_Null_When_No_SkipNullCheck_Defined_But_ParameterInfo_Passed() {
      var parameterInfo = new ParameterDetailsInfo { ParameterDetails = new List<ParameterDetails> { new ParameterDetails { Name = "test", Position = 0, SkipNullcheck = false } } };
      var inspector = new ValidatingParameterInspector(new[] { _singleValidatorMock.Object, _secondValidatorMock.Object, new NullCheckObjectValidator() }, _errorMessageGeneratorMock.Object, null, parameterInfo);
      object input = null;

      Assert.Throws<FaultException>(
        () => inspector.BeforeCall(OperationName, new[] { input }));
    }
  }
}
