# WCF Data Annotations - Extended, signed [![NuGet](https://img.shields.io/nuget/v/Independer.WCFDataAnnotations.svg)](https://www.nuget.org/packages/Independer.WCFDataAnnotations) 

WCFDataAnnotations allows you to automatically validate WCF service operation arguments using the attributes and IValidatableObject interface from System.ComponentModel.DataAnnotations.

## The purpose of this fork
The original version by DevTrends was a great starting point, however we needed some extra features like logging the validation exceptions and possibility of explicitly marking arguments allowing null to be passed to. 

The original source is still available on CodePlex, however CodePlex will be stopped and is in readonly mode already.

To continue this great initiative, we copied the CodePlex repository to GitHub and made our changes available for everyone.

## Improvements
The original validation logic doesn't cover every scenario:
* Nested IValidatableObjects (also within IEnumerables)
* Custom Validation Attributes

These issues have been fixed.

## New features

### IValidationResultsLogger
If you want to log the validation errors in your application you can add your implementation of the `IValidationResultsLogger`, so you can keep track of possible issues or mismatches between your client and server side validation.

#### Usage
After implementing the `IValidationResultsLogger` interface, all you need to do is defining that type for the `ValidateDataAnnotationsBehavior` attribute.

E.g.:
```csharp
[ValidateDataAnnotationsBehavior(typeof(ValidationResultsLogger))]
public class TestService : ITestService {

}
```

### [AllowNull] attribute
The library has a default null check which is performed always, 
regardless of the types of the arguments and therefor also for types that normally could have a null value (e.g.: classes, nullable types), 
but due to this restriction it is not possible to call any WCF method with null as argument. Even if the code could/should accept null values.

In general this restriction is fine to have, but in some cases it would be handy if we could override this behavior explicitly on certain arguments.

#### Usage
Usage is very simple, if you want to make one of your arguments nullable, just add the data annotation `[AllowNull]` to the argument in the interface of your wcf service. It is important to do that in the interface (ServiceContract) and not in the class itself, otherwise it won't work.

```csharp
[ServiceContract]
public interface ITestService {

  [OperationContract]
  string GetData(int value);

  [OperationContract]
  CompositeType GetDataUsingDataContract(CompositeType composite, [AllowNull] CompositeType nullableType);
}
```

## Strong Named (signed) version [![NuGet](https://img.shields.io/nuget/v/Independer.WCFDataAnnotations-Signed.svg)](https://www.nuget.org/packages/Independer.WCFDataAnnotations-Signed)
There is a signed version also available on NuGet which is identical to the normal package in terms of source code.

The `PublicKeyToken` of the signed version is: `656e010cba88e92b`

### Update (v1.3.0)
The original version of this package has been started as a non signed package and later on we introduced a signed version of it as a separate package with the only difference of having it signed.

We don't see any reason to keep these two versions as separate packages, so decided that we sign the main package also.

Therefore, from version `1.3.0` there is no point in keeping the `Independer.WCFDataAnnotations-Signed` package maintained anymore.

If you used that package previously, you can migrate and continue using the main package (`Independer.WCFDataAnnotations`) instead. 


## Version history

 * 1.3.0 
   * Enhancements:
     * Package is now signed
     * Migrated project to SDK-style
     * Target framework changes: 
       * Update .NET 4.5.1 target framework to .NET 4.7.2 
       * Add netstandard2.0 target for limited dotnetcore support ( validation logic, no WCF )
     * Added dotnet-pack.ps1 script. Usage: dotnet-pack.ps1 -signingKeyPath [PATH-TO-SNK-FILE]

 * 1.2.0.1 (Only for the Strong Named version)
   * 1.2.0 for Strong Named Nuget feed was published accidentally without being signed, so published 1.2.0.1 signed

 * 1.2.0 
   * Bug fixes: 
     * Fixed issue regarding using multiple endpoints with the same contract
     
 * 1.1.0 
   * Bug fixes: 
     * Fixed issue when injecting behavior with ServiceConfiguration in code
     
   * Enhancements: 
     * Added parameter name to validation results when missing 
     * Namespace change for better recognition of the fork in code
      
 * 1.0.0 
   * Initial version

## Credit
This repository is a fork of the DevTrends WCF DataAnnotations repository (https://wcfdataannotations.codeplex.com/ (https://archive.codeplex.com/?p=wcfdataannotations))

For more information about how DevTrends WCF Data Annotations worked and the design decisions behind it, please visit the: https://www.devtrends.co.uk/blog/validating-wcf-service-operations-using-system.componentmodel.dataannotations
