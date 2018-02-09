# WCF Data Annotations - With [AllowNull] attribute

WCFDataAnnotations allows you to automatically validate WCF service operation arguments using the attributes and IValidatableObject interface from System.ComponentModel.DataAnnotations.

## The purpose of this fork
In the original version by DevTrends there is a null check which is performed always, 
regardless of the types of the arguments and therefor also for types that normally could have a null value (e.g.: classes, nullable types), 
but due to this restriction it is not possible to call any WCF method with null as argument. Even if the code could/should accept null values.

In general this restriction is fine to have, but in some cases it would be handy if we could override this behavior explicitly on certain arguments.

## Usage
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

## Credit
This repository is a fork of the DevTrends WCF DataAnnotations repository (https://wcfdataannotations.codeplex.com/)
