﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NUnit.Framework;

namespace Independer.WCFDataAnnotations.UnitTests {
  [TestFixture]
  [Category(nameof(DataAnnotationsObjectValidatorTests))]
  public class DataAnnotationsObjectValidatorTests {
    private const string RequiredErrorMessage = "{0} is required";
    private const string RegexErrorMessage = "{0} can only contains numbers";
    private const string RangeErrorMessage = "{0} must be between {1} and {2}";

    private DataAnnotationsObjectValidator _validator;

    [SetUp]
    public void Setup() {
      _validator = new DataAnnotationsObjectValidator();
    }

    [Test]
    public void Validate_Does_Not_Return_ValidationResult_When_Passed_Object_With_No_DataAnnotations() {
      var result = _validator.Validate("blah");

      Assert.That(result, Is.Not.Null);
      Assert.That(result.Any(), Is.False);
    }

    [Test]
    public void Validate_Does_Not_Return_ValidationResult_When_Passed_Null() {
      var result = _validator.Validate(null);

      Assert.That(result, Is.Not.Null);
      Assert.That(result.Any(), Is.False);
    }

    [Test]
    public void Validate_Returns_ValidationResult_When_Passed_Object_Contains_Collection_With_Invalid_Properties() {
      var result = _validator.Validate(new List<TestClass>
      {
                new TestClass { Property1 = null, Property2 = "test" },
                new TestClass { Property1 = null, Property2 = "12345" },
                new TestClass { Property1 = "required", Property2 = "test" }
            });

      Assert.That(result.Count(), Is.EqualTo(4));
    }

    [Test]
    public void
        Validate_Returns_ValidationResult_When_Passed_Object_Contains_Nested_Collections_With_Invalid_Properties() {
      var result = _validator.Validate(new TestClass3 {
        Properties1 = null,
        Properties2 = new List<List<TestClass>>()
          {
                    new List<TestClass>
                    {
                        new TestClass
                        {
                            Property1 = null,
                            Property2 = "test"
                        },
                        new TestClass
                        {
                            Property1 = null,
                            Property2 = "12345"
                        },
                        new TestClass
                        {
                            Property1 = "required",
                            Property2 = "test"
                        }
                    },
                    new List<TestClass>
                    {
                        new TestClass
                        {
                            Property1 = "required",
                            Property2 = "12345"
                        },
                        new TestClass
                        {
                            Property1 = "required",
                            Property2 = "12345"
                        },
                        new TestClass
                        {
                            Property1 = "required",
                            Property2 = "12345"
                        },
                    }
                },
        Property3 = null
      }).ToList();

      Assert.That(result.Count, Is.EqualTo(6));

      Assert.That(result[0].ErrorMessage, Does.Contain(string.Format(RequiredErrorMessage, "Properties1")));
      Assert.That(result[1].ErrorMessage, Does.Contain(string.Format(RequiredErrorMessage, "Property1")));
      Assert.That(result[2].ErrorMessage, Does.Contain(string.Format(RegexErrorMessage, "Property2")));
      Assert.That(result[3].ErrorMessage, Does.Contain(string.Format(RequiredErrorMessage, "Property1")));
      Assert.That(result[4].ErrorMessage, Does.Contain(string.Format(RegexErrorMessage, "Property2")));
      Assert.That(result[5].ErrorMessage, Does.Contain(string.Format(RequiredErrorMessage, "Property3")));
    }

    [Test]
    public void Validate_Returns_ValidationResult_When_Passed_Object_That_Has_Invalid_Sub_Properties() {
      var result =
          _validator.Validate(new TestClass {
            Property1 = "test",
            Property2 = "12345",
            Property3 = new TestClass2 { Property3 = null, Property4 = 20 }
          }).ToList();

      Assert.That(result.Count, Is.EqualTo(2));
      Assert.That(result[0].ErrorMessage, Does.Contain(string.Format(RequiredErrorMessage, "Property3")));
      Assert.That(result[1].ErrorMessage, Does.Contain(string.Format(RangeErrorMessage, "Property4", 1, 10)));
    }

    [Test]
    public void Validate_Returns_ValidationResult_When_Passed_Object_That_Has_One_Invalid_Property() {
      var result = _validator.Validate(new TestClass { Property1 = null, Property2 = "12345" });

      Assert.That(result.Count(), Is.EqualTo(1));
      Assert.That(result.First().ErrorMessage, Does.Contain(string.Format(RequiredErrorMessage, "Property1")));
    }

    [Test]
    public void Validate_Returns_ValidationResult_When_Passed_Object_That_Has_Two_Invalid_Properties() {
      var result = _validator.Validate(new TestClass { Property1 = null, Property2 = "test" }).ToList();

      Assert.That(result.Count, Is.EqualTo(2));
      Assert.That(result[0].ErrorMessage, Does.Contain(string.Format(RequiredErrorMessage, "Property1")));
      Assert.That(result[1].ErrorMessage, Does.Contain(string.Format(RegexErrorMessage, "Property2")));
    }


    [Test]
    public void Validate_Custom_Validation_Attributes_Returns_ValidationResult_When_Passed_Object_Is_Invalid() {
      var result = _validator.Validate(new TestClass_With_Custom_Validation_attribute() );
      
      Assert.That(result, Is.Not.Null);
      Assert.That(result.Count(), Is.EqualTo(1));
      Assert.That(result.First().ErrorMessage, Does.Contain("ERROR ERROR"));
    }

    [Test]
    public void Validate_Enumerables_Returns_ValidationResult_When_Passed_IValidatableObject_That_Is_Not_Valid() {
      var testObject = new TestClass3 {
        Properties1 = new List<TestClass> {
          new TestClass { Property2 = "abz"}
        },
        Property3 = new TestClass2()
      };

      var result = _validator.Validate(testObject);

      Assert.That(result, Is.Not.Null);
      Assert.That(result.Count(), Is.EqualTo(4));
    }

    [Test]
    public void Validate_Nested_Enumerables_Returns_ValidationResult_When_Passed_IValidatableObject_That_Is_Not_Valid() {
      var testObject = new TestClass3 {
        Properties1 = new List<TestClass> {
          new TestClass { Property2 = "abz"}
        },
        Properties2 = new IEnumerable<TestClass>[] {
          new List<TestClass> {
            new TestClass(),
            new TestClass()
          },
        },
        Property3 = new TestClass2()
      };

      var result = _validator.Validate(testObject);

      Assert.That(result, Is.Not.Null);
      Assert.That(result.Count(), Is.EqualTo(6));
    }

    private class TestClass {
      [Required(ErrorMessage = RequiredErrorMessage)]
      public string Property1 { get; set; }

      [RegularExpression(@"\d{1,10}", ErrorMessage = RegexErrorMessage)]
      public string Property2 { get; set; }

      public TestClass2 Property3 { get; set; }
    }

    private class TestClass2 {
      [Required(ErrorMessage = RequiredErrorMessage)]
      public string Property3 { get; set; }

      [System.ComponentModel.DataAnnotations.Range(1, 10, ErrorMessage = RangeErrorMessage)]
      public int Property4 { get; set; }
    }

    private class TestClass3 {
      [Required(ErrorMessage = RequiredErrorMessage)]
      public IList<TestClass> Properties1 { get; set; }

      public IEnumerable<IEnumerable<TestClass>> Properties2 { get; set; }

      [Required(ErrorMessage = RequiredErrorMessage)]
      public TestClass2 Property3 { get; set; }
    }

    private class TestClass_With_Custom_Validation_attribute {
      [CustomRequiredValidator]
      public string SomeText { get; set; }
    }

    private class CustomRequiredValidator : ValidationAttribute {
      public override bool IsValid(object value) {
        return value != null;
      }

      protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
        return value != null ?
          ValidationResult.Success :
          new ValidationResult("ERROR ERROR");
      }
    }
  }
}