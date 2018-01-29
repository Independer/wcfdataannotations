using System.Collections.Generic;
using System.Linq;

namespace DevTrends.WCFDataAnnotations {
  public class ParameterInfo {
    public List<ParameterDetails> ParameterDetails { get; set; }

    public bool HasAnyParameterSkipNullCheck {
      get { return ParameterDetails.Any(x => x.SkipNullcheck); }
    }

    public ParameterInfo() {
      ParameterDetails = new List<ParameterDetails>();
    }
  }
}
