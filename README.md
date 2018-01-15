# WCF Data Annotations - With optional null check

WCFDataAnnotations allows you to automatically validate WCF service operation arguments using the attributes and IValidatableObject interface from System.ComponentModel.DataAnnotations.

## The purpose of this fork
In the original version by DevTrends there is a null check which is performed always, 
regardless of the types of the arguments and therefor also for types that normally could have a null value (e.g.: classes, nullable types), 
but due to this restriction it is not possible to call any WCF method with null as argument. Even if the code could/should accept null values.

In general this restriction is fine to have, but in some cases it would be handy if we could override this behavior explicitly on certain arguments.

## Status
Currently the mentioned feature is in progress, as soon as it is done, this readme will be updated.

## Credit
This repository is a fork of the DevTrends WCF DataAnnotations repository (https://wcfdataannotations.codeplex.com/)
