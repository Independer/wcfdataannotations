using System;

namespace DevTrends.WCFDataAnnotations {
  [AttributeUsage(AttributeTargets.Parameter)]
  public sealed class AllowNullAttribute: Attribute { }
}
