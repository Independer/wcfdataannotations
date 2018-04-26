using System;

namespace Independer.WCFDataAnnotations {
  [AttributeUsage(AttributeTargets.Parameter)]
  public sealed class AllowNullAttribute: Attribute { }
}
