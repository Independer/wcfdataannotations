using System;

namespace DevTrends.WCFDataAnnotations {
  [AttributeUsage(AttributeTargets.Parameter)]
  public sealed class SkipNullCheckAttribute: Attribute { }
}
